using SdHub.Database.Entities.Users;
using SdHub.Models;
using System.Threading.Tasks;
using System.Threading;
using System;
using SdHub.Shared.AspErrorHandling.ModelState;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SdHub.Constants;
using SdHub.Database;

namespace SdHub.Services.User;

public class UserService : IUserService
{
    private readonly SdHubDbContext _db;

    public UserService(SdHubDbContext db)
    {
        _db = db;
    }


    public async Task<UserEntity> AddUserAsync(UserEntity user, CancellationToken ct = default)
    {
        var ms = new ModelStateBuilder();

        var existedUser = await IncludeRequired(_db.Users)
            .IsDeleted(false)
            .FirstOrDefaultAsync(x => x.Guid == user.Guid, ct);
        if (existedUser != null)
            ms.AddError("UserExist").ThrowIfNotValid();

        //sanitize
        user.Id = 0;
        user.CreatedAt = DateTimeOffset.UtcNow;
        user.Guid = Guid.NewGuid();
        user.ApiTokens = null;
        user.RefreshTokens = null;
        user.Images = null;
        user.Plan = null;

        _db.Users.Add(user);
        await _db.SaveChangesAsync(CancellationToken.None);
        return user;
    }

    public async Task<UserEntity> SaveUserAsync(UserEntity user, CancellationToken ct = default)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync(CancellationToken.None);
        return user;
    }

    public Task<UserEntity> ConfirmEmailAsync(UserEntity user, CancellationToken ct = default)
    {
        user.EmailConfirmedAt ??= DateTimeOffset.UtcNow;
        return Task.FromResult(user);
    }

    public async Task<UserEntity> ConfirmEmailAsync(Guid guid, CancellationToken ct = default)
    {
        var user = await GetUserByGuidAsync(guid, ct);
        user = await ConfirmEmailAsync(user!, ct);
        user = await SaveUserAsync(user, ct);
        return user;
    }

    public async Task<bool> EmailIsUsedAsync(string? email, CancellationToken ct = default)
    {
        if (email == null)
            return true;
        email = email.Normalize().ToUpper();
        return await _db.Users.AnyAsync(x => x.EmailNormalized == email, ct);
    }

    public async Task<bool> LoginIsUsedAsync(string? login, CancellationToken ct = default)
    {
        if (login == null)
            return true;
        login = login.Normalize().ToUpper();
        return await _db.Users.AnyAsync(x => x.LoginNormalized == login, ct);
    }

    public async Task<PaginationResponse<UserEntity>> SearchUsersAsync(SearchUsersParams searchParams,
        CancellationToken ct = default)
    {
        var q = _db.Users.AsQueryable().IsDeleted(false);
        if (!string.IsNullOrWhiteSpace(searchParams.Query))
        {
            var query = searchParams.Query;
            var normalizedQuery = query.Normalize().ToUpper();
            var filtrateByGuid = Guid.TryParse(normalizedQuery, out var guid);
            q = q.Where(x => x.LoginNormalized!.Contains(normalizedQuery) ||
                             x.EmailNormalized!.Contains(normalizedQuery) ||
                             (filtrateByGuid && x.Guid == guid));
        }

        var total = await q.CountAsync(ct);

        var users = await IncludeRequired(q)
            .Skip(searchParams.Skip)
            .Take(searchParams.Count)
            .ToArrayAsync(ct);

        return new PaginationResponse<UserEntity>()
        {
            Take = users.Length,
            Skip = searchParams.Skip,
            Total = total,
            Values = users
        };
    }


    public async Task<UserEntity> UpdateUserAsync(UserUpdate req, Guid userGuid, CancellationToken ct = default)
    {
        var user = await GetUserByGuidAsync(userGuid, ct);
        var ms = new ModelStateBuilder();
        if (user == null)
            ms.AddError("UserNotFound").ThrowIfNotValid();

        user = await UpdateUserAsync(req, user!, ct);
        user = await SaveUserAsync(user, ct);
        return user;
    }

    public async Task<UserEntity> UpdateUserAsync(UserUpdate req, UserEntity user, CancellationToken ct = default)
    {
        var ms = new ModelStateBuilder();

        if (MustUpdate(req.PasswordHash, user.PasswordHash))
        {
            user.PasswordHash = req.PasswordHash;
        }

        if (MustUpdate(req.About, user.About))
        {
            user.About = req.About;
        }

        if (MustUpdate(req.PlanId, user.PlanId))
        {
            user.PlanId = req.PlanId!.Value;
        }

        var reqLoginNormalized = req.Login?.ToUpper().Normalize();
        if (MustUpdate(reqLoginNormalized, user.LoginNormalized))
        {
            if (await LoginIsUsedAsync(req.Login, ct))
                ms.AddError("Login used").ThrowIfNotValid();

            user.Login = req.Login!;
            user.LoginNormalized = reqLoginNormalized;
        }

        var reqEmailNormalized = req.Email?.ToUpper().Normalize();
        if (MustUpdate(reqEmailNormalized, user.EmailNormalized))
        {
            if (await EmailIsUsedAsync(req.Email, ct))
                ms.AddError("Email used").ThrowIfNotValid();

            user.Email = req.Email!;
            user.EmailNormalized = reqEmailNormalized;
        }

        if (req.Roles != null)
        {
            var newRoles = req.Roles.ToList();
            for (var i = 0; i < newRoles.Count; i++)
            {
                var parsedRole = UserRoleTypes.Parse(newRoles[i]);
                if (parsedRole == null)
                    ms.AddError("Role not found", newRoles[i]).ThrowIfNotValid();
                newRoles[i] = parsedRole!;
            }

            user.Roles = newRoles;
        }

        return user;
    }


    private bool MustUpdate<T>(T? newVal, T? oldVal)
    {
        if (newVal == null)
        {
            return false;
        }
        else if (oldVal == null)
        {
            return true;
        }
        else if (!Equals(oldVal, newVal))
        {
            return true;
        }

        return false;
    }

    public async Task<UserEntity?> GetUserByLoginOrEmailAsync(string? loginOrEmail, CancellationToken ct = default)
    {
        if (loginOrEmail == null)
            return null;
        loginOrEmail = loginOrEmail.Normalize().ToUpper();
        var user = await IncludeRequired(_db.Users)
            .AsNoTracking()
            .IsDeleted(false)
            .Where(x => x.EmailNormalized == loginOrEmail || x.LoginNormalized == loginOrEmail)
            .FirstOrDefaultAsync(ct);
        return user;
    }

    public async Task<UserEntity?> GetUserByIdAsync(long id, CancellationToken ct = default)
    {
        var user = await IncludeRequired(_db.Users)
            .AsNoTracking()
            .IsDeleted(false)
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(ct);
        return user;
    }

    public async Task<UserEntity?> GetUserByGuidAsync(Guid guid, CancellationToken ct = default)
    {
        var user = await IncludeRequired(_db.Users)
            .AsNoTracking()
            .IsDeleted(false)
            .Where(x => x.Guid == guid)
            .FirstOrDefaultAsync(ct);
        return user;
    }

    private IQueryable<UserEntity> IncludeRequired(IQueryable<UserEntity> query)
    {
        return query;
    }
}