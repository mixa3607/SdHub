using SdHub.Models.Enums;

namespace SdHub.Models.User;

public class SendResetPasswordEmailRequest
{
    public string? Login { get; set; }

    public CaptchaType CaptchaType { get; set; }
    public string? CaptchaCode { get; set; }
}