using Microsoft.EntityFrameworkCore;

namespace SdHub.Shared.EntityFramework;

public interface IDbSeeder<in T> where T : DbContext
{
    Task SeedAsync(T db, CancellationToken ct = default);
}
