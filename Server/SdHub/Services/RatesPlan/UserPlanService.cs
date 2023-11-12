using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SdHub.Constants;
using SdHub.Database;
using SdHub.Database.Entities.Users;

namespace SdHub.Services.RatesPlan;

public class UserPlanService : IUserPlanService
{
    private readonly SdHubDbContext _db;

    public UserPlanService(SdHubDbContext db)
    {
        _db = db;
    }

    public async Task<UserPlanEntity> GetDefaultAsync(CancellationToken ct = default)
    {
        return await _db.UserPlans.AsNoTracking().FirstAsync(x => x.Name == RatesPlanTypes.RegUserPlan, ct);
    }

    public async Task<UserPlanEntity> GetByNameAsync(string name, CancellationToken ct = default)
    {
        return await _db.UserPlans.AsNoTracking().FirstAsync(x => x.Name == name, ct);
    }

    public async Task<UserPlanEntity> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await _db.UserPlans.AsNoTracking().FirstAsync(x => x.Id == id, ct);
    }
}