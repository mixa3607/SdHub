using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SdHub.Constants;
using SdHub.Database.Entities.Files;
using SdHub.Database.Entities.Users;
using SdHub.Options;
using SdHub.Services.Tokens;
using SdHub.Shared.EntityFramework;
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

    public async Task SeedAsync(SdHubDbContext db, CancellationToken ct = default)
    {
        //file store
        if (!await db.FileStorages.AnyAsync(ct))
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
            await db.SaveChangesAsync(CancellationToken.None);
        }

        //plans
        var adminPlan = await db.UserPlans.FirstOrDefaultAsync(x => x.Name == RatesPlanTypes.AdminPlan, ct);
        
        var regUserPlan = await db.UserPlans.FirstOrDefaultAsync(x => x.Name == RatesPlanTypes.RegUserPlan, ct);

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
            adminPlan.MaxImagesPerUpload = 100;
            adminPlan.GridsPerHour = 200;
            adminPlan.MaxImagesPerGrid = 25000;
            adminPlan.MaxGridArchiveSizeUpload = 5L * 1024 * 1024; //5gb
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
            regUserPlan.MaxImagesPerUpload = 20;
            regUserPlan.GridsPerHour = 20;
            regUserPlan.MaxImagesPerGrid = 2500;
            regUserPlan.MaxGridArchiveSizeUpload = 1L * 1024 * 1024; //1gb
        }
        await db.SaveChangesAsync(CancellationToken.None);

        //users
        if (!await db.Users.AnyAsync(x => x.Roles.Contains(UserRoleTypes.Admin), ct))
        {
            _logger.LogInformation("Create admin user");
            var user = new UserEntity()
            {
                Roles = new List<string>() { UserRoleTypes.Admin, UserRoleTypes.User, UserRoleTypes.HangfireRW },
                Login = "Admin",
                PasswordHash = _passwordService.CreatePasswordHash(_options.AdminPassword!),
                Plan = await db.UserPlans.FirstAsync(x => x.Name == RatesPlanTypes.AdminPlan, ct),
                Email = "admin@test.com",
                EmailConfirmedAt = DateTimeOffset.UtcNow,
            };
            db.Users.Add(user);
            await db.SaveChangesAsync(CancellationToken.None);
        }
    }
}