using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SdHub.Constants;
using SdHub.Database;
using SdHub.Database.Entities.Users;
using SdHub.Services.Tokens;

namespace SdHub.Hangfire.BasicAuth;

public class HangfireUsersService : IHangfireUsersService
{
    private readonly SdHubDbContext _db;
    private readonly IUserPasswordService _passwordService;

    public HangfireUsersService(SdHubDbContext db, IUserPasswordService passwordService)
    {
        _db = db;
        _passwordService = passwordService;
    }

    public bool IsPasswordCorrect(UserEntity user, string password)
    {
        return _passwordService.Validate(password, user.PasswordHash);
    }

    public Task<UserEntity?> GetUserByNameAsync(string name, CancellationToken ct = default)
    {
        return _db.Users.FirstOrDefaultAsync(
            x => x.LoginNormalized == name.Normalize().ToUpper()
                 && (x.Roles.Contains(UserRoleTypes.HangfireRO) || x.Roles.Contains(UserRoleTypes.HangfireRW))
                 && x.DeletedAt == null, ct);
    }
}