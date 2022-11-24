using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SdHub.Services.ErrorHandling.Handlers;

namespace SdHub.Hangfire;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHangfireBackgroundJobs(this IServiceCollection services,
        params Assembly[] assemblies)
    {
        var types = assemblies.SelectMany(x => x.ExportedTypes.Where(y =>
                y.GetInterfaces().Contains(typeof(IServerExceptionHandler)))
            )
            .Where(x => services.All(y => y.ServiceType != x))
            .ToArray();

        foreach (var type in types)
        {
            services.AddSingleton(typeof(IServerExceptionHandler), type);
        }

        return services;
    }

    public static IServiceCollection AddHangfireRecurrentJobs(this IServiceCollection services,
        params Assembly[] assemblies)
    {
        var types = assemblies.SelectMany(x => x.ExportedTypes.Where(y =>
                y.GetInterfaces().Contains(typeof(IServerExceptionHandler)))
            )
            .Where(x => services.All(y => y.ServiceType != x))
            .ToArray();

        foreach (var type in types)
        {
            services.AddSingleton(typeof(IServerExceptionHandler), type);
        }

        return services;
    }
}