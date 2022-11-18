using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SdHub.Database;
using SdHub.Database.Entities.User;
using SdHub.Options;

namespace SdHub.Services.Tokens;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly SdHubDbContext _db;
    private readonly IOptions<WebSecurityOptions> _options;

    public RefreshTokenService(SdHubDbContext db, IOptions<WebSecurityOptions> options)
    {
        _db = db;
        _options = options;
    }

    public async Task<RefreshTokenEntity> CreateNewAsync(Guid userGuid, string ip, string userAgent, CancellationToken ct = default)
    {
        var token = "rf_" + Guid.NewGuid().ToString("N");
        var expired = DateTimeOffset.UtcNow.Add(_options.Value.Jwt.RefreshTokenLifetime);
        var obj = new RefreshTokenEntity()
        {
            CreatedAt = DateTimeOffset.UtcNow,
            IpAddress = ip,
            UserGuid = userGuid,
            UserAgent = userAgent,
            Token = token,
            ExpiredAt = expired,
            UsedAt = default,
            Id = default
        };
        _db.RefreshTokens.Add(obj);
        await _db.SaveChangesAsync(CancellationToken.None);
        return obj;
    }

    public async Task<RefreshTokenEntity?> GetAsync(string? token, CancellationToken ct = default)
    {
        if (token == null)
            return null;
        var entity = await _db.RefreshTokens
            .Where(x => x.Token == token)
            .FirstOrDefaultAsync(CancellationToken.None);
        return entity;
    }

    public async Task<bool> MarkAsUsedAsync(string token, CancellationToken ct = default)
    {
        var entity = await _db.RefreshTokens
            .Where(x => x.UsedAt == null && x.Token == token)
            .FirstOrDefaultAsync(CancellationToken.None);
        if (entity == null)
            return false;

        entity.UsedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(CancellationToken.None);
        return true;
    }
}