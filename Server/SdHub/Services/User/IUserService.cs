using System;
using System.Threading;
using System.Threading.Tasks;
using SdHub.Database.Entities.Users;
using SdHub.Models;

namespace SdHub.Services.User;

public interface IUserService
{
    Task<UserEntity?> GetUserByLoginOrEmailAsync(string? login, CancellationToken ct = default);
    Task<UserEntity?> GetUserByIdAsync(long id, CancellationToken ct = default);
    Task<UserEntity?> GetUserByGuidAsync(Guid guid, CancellationToken ct = default);

    Task<UserEntity> AddUserAsync(UserEntity user, CancellationToken ct = default);
    Task<UserEntity> SaveUserAsync(UserEntity user, CancellationToken ct = default);
    Task<UserEntity> ConfirmEmailAsync(UserEntity user, CancellationToken ct = default);
    Task<UserEntity> ConfirmEmailAsync(Guid guid, CancellationToken ct = default);

    /// <summary>
    /// Сразу сохраняет в БД!
    /// </summary>
    Task<UserEntity> UpdateUserAsync(UserUpdate req, Guid userGuid, CancellationToken ct = default);

    /// <summary>
    /// Не сохраняет в бд
    /// </summary>
    Task<UserEntity> UpdateUserAsync(UserUpdate req, UserEntity user, CancellationToken ct = default);

    Task<bool> EmailIsUsedAsync(string email, CancellationToken ct = default);
    Task<bool> LoginIsUsedAsync(string login, CancellationToken ct = default);

    Task<PaginationResponse<UserEntity>> SearchUsersAsync(SearchUsersParams searchParams,
        CancellationToken ct = default);

    Task<PaginationResponse<UserApiTokenEntity>> GetApiTokensAsync(Guid userGuid, CancellationToken ct = default);
    Task<UserApiTokenEntity> AddApiTokenAsync(Guid userGuid, string name, DateTimeOffset expiredAt, CancellationToken ct = default);
    Task<UserApiTokenEntity> GetApiTokenByNameAsync(string name, CancellationToken ct = default);
    Task<UserApiTokenEntity> GetApiTokenByTokenAsync(string token, CancellationToken ct = default);
    Task<UserApiTokenEntity> RevokeApiTokenAsync(string name, CancellationToken ct = default);
}