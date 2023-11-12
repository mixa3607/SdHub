using System.Threading;
using System.Threading.Tasks;
using SdHub.Database.Entities.Users;

namespace SdHub.Services.RatesPlan;

public interface IUserPlanService
{
    Task<UserPlanEntity> GetDefaultAsync(CancellationToken ct = default);
    Task<UserPlanEntity> GetByNameAsync(string name, CancellationToken ct = default);
    Task<UserPlanEntity> GetByIdAsync(long id, CancellationToken ct = default);
}