using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SdHub.Database;
using SdHub.Database.Entities.Users;
using SdHub.Models.Enums;
using SdHub.Options;

namespace SdHub.Services.Refresh;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly SdHubDbContext _db;
    private readonly IOptions<WebSecurityOptions> _options;
    private readonly ILogger<RefreshTokenEntity> _logger;

    public RefreshTokenService(SdHubDbContext db, IOptions<WebSecurityOptions> options,
        ILogger<RefreshTokenEntity> logger)
    {
        _db = db;
        _options = options;
        _logger = logger;
    }

    public async Task<RefreshTokenEntity> CreateNewAsync(Guid userGuid, IReadOnlyList<AudienceType> aud,
        string fingerprint, string userAgent, CancellationToken ct = default)
    {
        var token = "rf_" + Guid.NewGuid().ToString("N");
        var expired = DateTimeOffset.UtcNow.Add(_options.Value.Jwt.RefreshTokenLifetime);
        var obj = new RefreshTokenEntity()
        {
            Audiences = aud.ToList(),
            CreatedAt = DateTimeOffset.UtcNow,
            Fingerprint = fingerprint,
            UserAgent = userAgent,
            Token = token,
            ExpiredAt = expired,
            UserGuid = userGuid
        };
        _db.RefreshTokens.Add(obj);
        await _db.SaveChangesAsync(CancellationToken.None);
        return obj;
    }

    public async Task<RefreshTokenEntity?> GetAsync(string? token, CancellationToken ct = default)
    {
        if (token == null)
            return null;
        var entity = await _db.RefreshTokens.Where(x => x.Token == token).FirstOrDefaultAsync(ct);
        return entity;
    }

    public async Task<bool> MarkAsUsedAsync(string token, CancellationToken ct = default)
    {
        var entity = await _db.RefreshTokens.Where(x => x.UsedAt == null && x.Token == token).FirstOrDefaultAsync(ct);
        if (entity == null)
        {
            _logger.LogError("Refresh token {token} not found in db", token);
            return false;
        }

        entity.UsedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(CancellationToken.None);
        return true;
    }
}