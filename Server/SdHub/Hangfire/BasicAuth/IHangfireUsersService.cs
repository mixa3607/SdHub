using System.Threading;
using System.Threading.Tasks;
using SdHub.Database.Entities.User;

namespace SdHub.Hangfire.BasicAuth;

public interface IHangfireUsersService
{
    bool IsPasswordCorrect(UserEntity user, string password);
    Task<UserEntity?> GetUserByNameAsync(string name, CancellationToken ct = default);
}