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
        if (!await db.UserPlans.AnyAsync(x => x.Name == RatesPlanTypes.AdminPlan))
        {
            var plan = new UserPlanEntity()
            {
                Name = RatesPlanTypes.AdminPlan,
                OnlyWithMetadata = false,

                ImagesPerHour = 5000,
                MaxImageSizeUpload = 15 * 1024,
                ImagesPerUpload = 100,

                GridsPerHour = 200,
                ImagesPerGrid = 25000,
                MaxGridArchiveSizeUpload = 5L * 1024 * 1024, //5gb
            };
            db.UserPlans.Add(plan);
            await db.SaveChangesAsync();
        }

        if (!await db.UserPlans.AnyAsync(x => x.Name == RatesPlanTypes.AnonUserPlan))
        {
            var plan = new UserPlanEntity()
            {
                Name = RatesPlanTypes.AnonUserPlan,
                OnlyWithMetadata = true,

                ImagesPerHour = 50,
                MaxImageSizeUpload = 3 * 1024,
                ImagesPerUpload = 10,

                GridsPerHour = 20,
                ImagesPerGrid = 2500,
                MaxGridArchiveSizeUpload = 1L * 1024 * 1024, //1gb
            };
            db.UserPlans.Add(plan);
            await db.SaveChangesAsync();
        }

        if (!await db.UserPlans.AnyAsync(x => x.Name == RatesPlanTypes.RegUserPlan))
        {
            var plan = new UserPlanEntity()
            {
                Name = RatesPlanTypes.RegUserPlan,
                OnlyWithMetadata = false,

                ImagesPerHour = 500,
                MaxImageSizeUpload = 5 * 1024,
                ImagesPerUpload = 20,

                GridsPerHour = 0,
                ImagesPerGrid = 0,
                MaxGridArchiveSizeUpload = 0
            };
            db.UserPlans.Add(plan);
            await db.SaveChangesAsync();
        }

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