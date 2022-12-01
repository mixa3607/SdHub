namespace SdHub.Options;

public class RecaptchaOptions
{
    public string? SecretKey { get; set; }
    public string? SiteKey { get; set; }
    public bool Bypass { get; set; }
}