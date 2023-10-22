using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SdHub.Shared.AspErrorHandling.Handlers;

namespace SdHub.Shared.AspErrorHandling;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRbErrorHandlersBuiltin(this IServiceCollection services)
    {
        return services.Scan(x => x
                .FromAssemblies(typeof(IServerExceptionHandler).Assembly)
                .AddClasses(c => c.AssignableTo<IServerExceptionHandler>())
                .As<IServerExceptionHandler>()
                .WithSingletonLifetime())
            ;
    }

    public static IServiceCollection AddRbErrorHandlersFromAssembly(this IServiceCollection services,
        Assembly assembly)
    {
        return services.Scan(x => x
                .FromAssemblies(assembly)
                .AddClasses(c => c.AssignableTo<IServerExceptionHandler>())
                .As<IServerExceptionHandler>()
                .WithSingletonLifetime())
            ;
    }
}
