using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SdHub.Database.Shared;

public class DbMigrator<T> : IDbMigrator<T> where T : DbContext
{
    private readonly ILogger<DbMigrator<T>> _logger;
    private readonly IDbSeeder<T>? _seeder;
    private readonly T _db;

    public DbMigrator(IDbSeeder<T>? seeder, T db, ILogger<DbMigrator<T>> logger)
    {
        _seeder = seeder;
        _db = db;
        _logger = logger;
    }

    public async Task<string[]> GetPendingMigrations()
    {
        return (await _db.Database.GetPendingMigrationsAsync()).ToArray();
    }

    public async Task Migrate(CancellationToken ct = default)
    {
        _logger.LogInformation("Begin migrating");
        await _db.Database.MigrateAsync(ct);
        _logger.LogInformation("Successfully migrated");
    }

    public async Task Seed()
    {
        if (_seeder != null)
        {
            _logger.LogInformation("Begin seeding");
            await _seeder.Seed(_db);
            _logger.LogInformation("Successfully seed");
        }
        else
        {
            _logger.LogWarning("Seeder for db not found");
        }
    }
}