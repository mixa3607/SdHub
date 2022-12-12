namespace SdHub.Models;

public class FrontendSettings
{
    public string? RecaptchaSiteKey { get; set; }

    public bool DisableUsersRegistration { get; set; }
    public bool DisableCaptcha { get; set; }
    public bool DisableImageUploadAnon { get; set; }
    public bool DisableImageUploadAuth { get; set; }
    public bool DisableGridUploadAuth { get; set; }
}