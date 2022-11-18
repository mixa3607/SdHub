using System.Linq;
using SdHub.Services.ErrorHandling.Handlers;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace SdHub.Services.ErrorHandling.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomErrorHandling(this IServiceCollection services,
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