using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SdHub.Shared.AspValidation;

public class StartupValidatorsCheckHelper
{
    public static void CheckActionsValidators(IServiceProvider serviceProvider, bool onlyOnDevEnv = true)
    {
        var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
        var logger = serviceProvider.GetRequiredService<ILogger<StartupValidatorsCheckHelper>>();
        if (onlyOnDevEnv && !environment.IsDevelopment())
        {
            logger.LogInformation("Skip validators check in non Development env");
            return;
        }

        var scope = serviceProvider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<AllRequiredValidatorsRegisteredService>();
        service.CheckControllersParams();
    }

    /// <summary>
    /// Check that process run in required tz. Ignored in Development
    /// </summary>
    /// <param name="services"></param>
    /// <param name="requiredOffset">If null used +0</param>
    public static IServiceProvider CheckTz(IServiceProvider services, TimeSpan? requiredOffset = null)
    {
        var logger = services.GetRequiredService<ILogger<StartupValidatorsCheckHelper>>();
        requiredOffset ??= TimeSpan.FromHours(0);
        var tz = TimeZoneInfo.Local;

        if (tz.BaseUtcOffset == requiredOffset)
            return services;

        if (!services.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
        {
            logger.LogCritical("Detect TZ diff! Must run with utc+{h}", requiredOffset.Value.Hours);
            throw new Exception($"Detect TZ diff! Must run with utc+{requiredOffset.Value.Hours}");
        }

        logger.LogError("Detect TZ diff! Ignore bcs in Development env");
        return services;
    }

    /// <summary>
    /// Check that process run with required locale. Ignored in Development
    /// </summary>
    /// <param name="services"></param>
    /// <param name="requiredCulture">If null used Invariant</param>
    public static IServiceProvider CheckLocale(IServiceProvider services, CultureInfo? requiredCulture = null)
    {
        var logger = services.GetRequiredService<ILogger<StartupValidatorsCheckHelper>>();
        requiredCulture ??= CultureInfo.InvariantCulture;
        var culture = CultureInfo.CurrentCulture;

        if (culture.Equals(requiredCulture))
            return services;

        if (!services.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
        {
            logger.LogCritical("Detect locale mismatch! Must run with {locale} locale", requiredCulture.DisplayName);
            throw new Exception($"Detect locale mismatch! Must run with {requiredCulture.DisplayName} locale");
        }

        logger.LogError("Detect locale mismatch! Ignore bcs in Development env");
        return services;
    }
}

