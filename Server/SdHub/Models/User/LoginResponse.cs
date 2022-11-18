using System;

namespace SdHub.Models.User;

public class LoginResponse
{
    public string? JwtToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTimeOffset RefreshTokenExpired { get; set; }
    public UserModel? User { get; set; }
}