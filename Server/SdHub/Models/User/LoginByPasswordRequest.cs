using SdHub.Models.Enums;

namespace SdHub.Models.User;

public class LoginByPasswordRequest
{
    public string? Login { get; set; }
    public string? Password { get; set; }

    public CaptchaType CaptchaType { get; set; }
    public string? CaptchaCode { get; set; }
}