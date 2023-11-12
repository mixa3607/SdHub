using SdHub.Models.Enums;

namespace SdHub.Models.User;

public class RegisterRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Login { get; set; }

    public CaptchaType CaptchaType { get; set; }
    public string? CaptchaCode { get; set; }
}