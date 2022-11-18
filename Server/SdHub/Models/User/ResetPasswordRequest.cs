using SdHub.Models.Enums;

namespace SdHub.Models.User;

public class ResetPasswordRequest
{
    public string? Email { get; set; }

    public CaptchaType CaptchaType { get; set; }
    public string? CaptchaCode { get; set; }
}