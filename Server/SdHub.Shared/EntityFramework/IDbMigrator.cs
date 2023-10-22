using Microsoft.EntityFrameworkCore;

namespace SdHub.Shared.EntityFramework;

public interface IDbMigrator
{
    Task<string[]> GetPendingMigrationsAsync();
    Task MigrateAsync(CancellationToken ct = default);
    Task SeedAsync(CancellationToken ct = default);
    DbContext DbContext { get; }
}

public interface IDbMigrator<T> : IDbMigrator where T : DbContext
{
    public new T DbContext { get; }
}
