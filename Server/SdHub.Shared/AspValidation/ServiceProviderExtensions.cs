using System.Globalization;

namespace SdHub.Shared.AspValidation;

public static class ServiceProviderExtensions
{
    /// <inheritdoc cref="StartupValidatorsCheckHelper.CheckActionsValidators"/>
    public static IServiceProvider RbCheckActionsValidators(this IServiceProvider services, bool onlyOnDevEnv = true)
    {
        StartupValidatorsCheckHelper.CheckActionsValidators(services, onlyOnDevEnv);
        return services;
    }

    /// <inheritdoc cref="StartupValidatorsCheckHelper.CheckTz"/>
    public static IServiceProvider RbCheckTz(this IServiceProvider services, TimeSpan? requiredOffset = null)
    {
        StartupValidatorsCheckHelper.CheckTz(services, requiredOffset);
        return services;
    }

    /// <inheritdoc cref="StartupValidatorsCheckHelper.CheckLocale"/>
    public static IServiceProvider RbCheckLocale(this IServiceProvider services, CultureInfo? requiredCulture = null)
    {
        StartupValidatorsCheckHelper.CheckLocale(services, requiredCulture);
        return services;
    }
}
