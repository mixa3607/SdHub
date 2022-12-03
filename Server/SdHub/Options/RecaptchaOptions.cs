using System.ComponentModel.DataAnnotations;

namespace SdHub.Options;

/// <summary>
/// Google recaptcha options
/// </summary>
public class RecaptchaOptions
{
    /// <summary>
    /// Disable captcha
    /// </summary>
    public bool Bypass { get; set; } = true;

    /// <summary>
    /// Secret key
    /// </summary>
    [Required]
    public string? SecretKey { get; set; }

    /// <summary>
    /// Public site key
    /// </summary>
    [Required]
    public string? SiteKey { get; set; }
}