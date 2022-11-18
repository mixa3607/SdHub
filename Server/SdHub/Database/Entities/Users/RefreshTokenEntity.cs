using System;

namespace SdHub.Database.Entities.User;

public class RefreshTokenEntity
{
    public long Id { get; set; }
    public string? Token { get; set; }

    public Guid UserGuid { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset ExpiredAt { get; set; }
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }
    public DateTimeOffset? UsedAt { get; set; }
}