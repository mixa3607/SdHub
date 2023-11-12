using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SdHub.Database.Entities.Users;
using SdHub.Models.Enums;

namespace SdHub.Services.Refresh;

public interface IRefreshTokenService
{
    Task<RefreshTokenEntity> CreateNewAsync(Guid userGuid, IReadOnlyList<AudienceType> aud, string fingerprint,
        string userAgent, CancellationToken ct = default);

    Task<RefreshTokenEntity?> GetAsync(string? token, CancellationToken ct = default);

    Task<bool> MarkAsUsedAsync(string token, CancellationToken ct = default);
}