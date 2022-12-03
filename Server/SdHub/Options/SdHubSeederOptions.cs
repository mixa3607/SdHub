using System.ComponentModel.DataAnnotations;

namespace SdHub.Options;

/// <summary>
/// Database seeder options
/// </summary>
public class SdHubSeederOptions
{
    /// <summary>
    /// Password for Admin account
    /// </summary>
    [Required]
    public string? AdminPassword { get; set; }
}