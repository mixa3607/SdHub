using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SdHub.Shared.EntityFramework;

public static class AspExtensions
{
    public static IServiceCollection AddRbDbMigrator<T>(this IServiceCollection services) where T : DbContext
    {
        services
            .AddScoped<IDbMigrator>(x => x.GetRequiredService<IDbMigrator<T>>())
            .AddScoped<IDbMigrator<T>, DbMigrator<T>>();
        return services;
    }

    public static async Task<T> RbEfMigrateAsync<TDb, T>(this T host, CancellationToken ct = default)
        where T : IHost where TDb : DbContext
    {
        await EfStartupMigrationHelper.MigrateAsync<TDb>(host.Services, ct);
        return host;
    }

    public static async Task<IServiceProvider> RbEfMigrateAsync<T>(this IServiceProvider services,
        CancellationToken ct = default) where T : DbContext
    {
        await EfStartupMigrationHelper.MigrateAsync<T>(services, ct);
        return services;
    }
}
