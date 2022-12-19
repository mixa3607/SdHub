using System;

namespace SdHub.Models.User;

public class UserApiTokenModel
{
    public string? Token { get; set; }
    public DateTimeOffset ExpiredAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
}