using System;

namespace SdHub.Database.Entities.Users;

public class UserApiTokenEntity
{
    public string Token { get; set; } = null!;
    public string Name { get; set; } = null!;

    public DateTimeOffset ExpiredAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public long UserId { get; set; }
    public UserEntity? User { get; set; }
}