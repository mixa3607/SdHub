using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SdHub.Shared.EntityFramework;

public class DbMigrator<T> : IDbMigrator<T> where T : DbContext
{
    private readonly ILogger<DbMigrator<T>> _logger;
    private readonly IDbSeeder<T>? _seeder;
    private readonly T _db;

    public T DbContext => _db;
    DbContext IDbMigrator.DbContext => _db;

    public DbMigrator(T db, ILogger<DbMigrator<T>> logger, IDbSeeder<T>? seeder = null)
    {
        _seeder = seeder;
        _db = db;
        _logger = logger;
    }

    public async Task<string[]> GetPendingMigrationsAsync()
    {
        return (await _db.Database.GetPendingMigrationsAsync()).ToArray();
    }

    public async Task MigrateAsync(CancellationToken ct = default)
    {
        var pendingMigrations = await GetPendingMigrationsAsync();
        if (pendingMigrations.Length == 0)
        {
            _logger.LogInformation("No pending migrations");
            return;
        }

        _logger.LogInformation("Begin applying pending migrations {pending}", (object)pendingMigrations);
        await _db.Database.MigrateAsync(ct);
        _logger.LogInformation("Successfully migrated");
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        if (_seeder != null)
        {
            _logger.LogInformation("Begin seeding");
            await _seeder.SeedAsync(_db, ct);
            _logger.LogInformation("Successfully seed");
        }
        else
        {
            _logger.LogInformation("Seeder for db not found");
        }
    }
}
