using System;
using System.Threading;
using System.Threading.Tasks;
using SdHub.Database.Entities.User;

namespace SdHub.Services.Tokens;

public interface IRefreshTokenService
{
    Task<RefreshTokenEntity> CreateNewAsync(Guid userGuid, 
        string ip, string userAgent, CancellationToken ct = default);

    Task<RefreshTokenEntity?> GetAsync(string? token, CancellationToken ct = default);

    Task<bool> MarkAsUsedAsync(string token, CancellationToken ct = default);
}