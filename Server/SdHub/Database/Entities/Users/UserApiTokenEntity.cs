using System;

namespace SdHub.Database.Entities.Users;

public class UserApiTokenEntity
{
    public string? Token { get; set; }
    public DateTimeOffset ExpiredAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

    public long UserId { get; set; }
    public UserEntity? User { get; set; }
}