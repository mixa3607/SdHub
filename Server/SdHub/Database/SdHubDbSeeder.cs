using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SdHub.Constants;
using SdHub.Database.Entities.Files;
using SdHub.Database.Entities.Users;
using SdHub.Database.Shared;
using SdHub.Options;
using SdHub.Services.Tokens;
using SdHub.Storage;
using SdHub.Storage.Local;

namespace SdHub.Database;

public class SdHubDbSeeder : IDbSeeder<SdHubDbContext>
{
    private readonly ILogger<SdHubDbSeeder> _logger;
    private readonly IUserPasswordService _passwordService;
    private readonly AppInfoOptions _appInfo;
    private readonly SdHubSeederOptions _options;

    public SdHubDbSeeder(ILogger<SdHubDbSeeder> logger, IUserPasswordService passwordService, IOptions<AppInfoOptions> appInfo, IOptions<SdHubSeederOptions> options)
    {
        _logger = logger;
        _passwordService = passwordService;
        _options = options.Value;
        _appInfo = appInfo.Value;
    }

    public async Task Seed(SdHubDbContext db)
    {
        //file store
        if (!await db.FileStorages.AnyAsync())
        {
            var settings = new LocalStorageSettings()
            {
                PhysicalRoot= "./wwwroot/images",
                VirtualRoot = "./images",
                TempPath = "./wwwroot/temp"
            };
            var store = new FileStorageEntity()
            {
                Name = "Local1",
                BaseUrl = _appInfo.BaseUrl,
                BackendType = FileStorageBackendType.LocalDir,
                Settings = settings.Save(),
            };
            db.FileStorages.Add(store);
            await db.SaveChangesAsync();
        }

        //plans
        var adminPlan = await db.UserPlans.FirstOrDefaultAsync(x => x.Name == RatesPlanTypes.AdminPlan);
        
        var anonPlan = await db.UserPlans.FirstOrDefaultAsync(x => x.Name == RatesPlanTypes.AnonUserPlan);
        var regUserPlan = await db.UserPlans.FirstOrDefaultAsync(x => x.Name == RatesPlanTypes.RegUserPlan);

        if (adminPlan == null || _options.OverwriteUserPlans)
        {
            if (adminPlan == null)
            {
                adminPlan = new UserPlanEntity();
                db.UserPlans.Add(adminPlan);
            }

            adminPlan.Name = RatesPlanTypes.AdminPlan;
            adminPlan.OnlyWithMetadata = false;
            adminPlan.ImagesPerHour = 5000;
            adminPlan.MaxImageSizeUpload = 15 * 1024;
            adminPlan.ImagesPerUpload = 100;
            adminPlan.GridsPerHour = 200;
            adminPlan.ImagesPerGrid = 25000;
            adminPlan.MaxGridArchiveSizeUpload = 5L * 1024 * 1024; //5gb
        }

        if (anonPlan == null || _options.OverwriteUserPlans)
        {
            if (anonPlan == null)
            {
                anonPlan = new UserPlanEntity();
                db.UserPlans.Add(anonPlan);
            }
            anonPlan.Name = RatesPlanTypes.AnonUserPlan;
            anonPlan.OnlyWithMetadata = true;
            anonPlan.ImagesPerHour = 50;
            anonPlan.MaxImageSizeUpload = 3 * 1024;
            anonPlan.ImagesPerUpload = 10;
            anonPlan.GridsPerHour = 0;
            anonPlan.ImagesPerGrid = 0;
            anonPlan.MaxGridArchiveSizeUpload = 0;
        }

        if (regUserPlan == null || _options.OverwriteUserPlans)
        {
            if (regUserPlan == null)
            {
                regUserPlan = new UserPlanEntity();
                db.UserPlans.Add(regUserPlan);
            }

            regUserPlan.Name = RatesPlanTypes.RegUserPlan;
            regUserPlan.OnlyWithMetadata = false;
            regUserPlan.ImagesPerHour = 500;
            regUserPlan.MaxImageSizeUpload = 10 * 1024;
            regUserPlan.ImagesPerUpload = 20;
            regUserPlan.GridsPerHour = 20;
            regUserPlan.ImagesPerGrid = 2500;
            regUserPlan.MaxGridArchiveSizeUpload = 1L * 1024 * 1024; //1gb
        }
        await db.SaveChangesAsync();

        //users
        if (!await db.Users.AnyAsync(x => x.Roles.Contains(UserRoleTypes.Admin)))
        {
            _logger.LogInformation("Create admin user");
            var user = new UserEntity()
            {
                Roles = new List<string>() { UserRoleTypes.Admin, UserRoleTypes.User, UserRoleTypes.HangfireRW },
                Login = "Admin",
                PasswordHash = _passwordService.CreatePasswordHash(_options.AdminPassword!),
                Plan = await db.UserPlans.FirstAsync(x => x.Name == RatesPlanTypes.AdminPlan),
                Email = "admin@test.com",
                EmailConfirmedAt = DateTimeOffset.Now,
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }
        if (await db.Users.AllAsync(x => !x.IsAnonymous))
        {
            _logger.LogInformation("Create anon user");
            var user = new UserEntity()
            {
                Roles = new List<string>() { UserRoleTypes.User },
                Login = "Anon",
                PasswordHash = "",
                Plan = await db.UserPlans.FirstAsync(x => x.Name == RatesPlanTypes.AnonUserPlan),
                IsAnonymous = true
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }
    }
}