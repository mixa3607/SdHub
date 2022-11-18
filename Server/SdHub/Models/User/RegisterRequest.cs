using SdHub.Models.Enums;

namespace SdHub.Models.User;

public class RegisterRequest
{
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Login { get; set; }

    public CaptchaType CaptchaType { get; set; }
    public string? CaptchaCode { get; set; }
}