using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SdHub.Database.Shared;

public interface IDbMigrator<T> where T : DbContext
{
    Task<string[]> GetPendingMigrations();
    Task Migrate(CancellationToken ct = default);
    Task Seed();
}