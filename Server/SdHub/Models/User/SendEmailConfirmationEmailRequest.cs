using SdHub.Models.Enums;

namespace SdHub.Models.User;

public class SendEmailConfirmationEmailRequest
{
    public string? Login { get; set; }

    public CaptchaType CaptchaType { get; set; }
    public string? CaptchaCode { get; set; }
}