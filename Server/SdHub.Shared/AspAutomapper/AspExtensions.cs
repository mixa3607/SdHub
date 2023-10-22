using Microsoft.Extensions.Hosting;

namespace SdHub.Shared.AspAutomapper;

public static class AspExtensions
{
    public static T RbValidateAutomapper<T>(this T host) where T : IHost
    {
        RbAutomapperValidationHelper.ValidateAutomapper(host.Services);
        return host;
    }

    public static IServiceProvider RbValidateAutomapper(this IServiceProvider serviceProvider)
    {
        RbAutomapperValidationHelper.ValidateAutomapper(serviceProvider);
        return serviceProvider;
    }
}
