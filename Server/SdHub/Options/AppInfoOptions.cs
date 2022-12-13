using System.ComponentModel.DataAnnotations;

namespace SdHub.Options;

/// <summary>
/// Application info
/// </summary>
public class AppInfoOptions
{
    /// <summary>
    /// Base url
    /// </summary>
    [Required]
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Git ref. Fill from env if docker container
    /// </summary>
    [Required]
    public string? GitRefName { get; set; } = "not_set";

    /// <summary>
    /// Git sha. Fill from env if docker container
    /// </summary>
    [Required]
    public string? GitCommitSha { get; set; } = "deadbeef";

    /// <summary>
    /// Use angular dev server instead compiled blobs. For development
    /// </summary>
    public string? FrontDevServer { get; set; }

    /// <summary>
    /// Disable user registration
    /// </summary>
    public bool DisableUsersRegistration { get; set; } = true;

    public bool DisableImageUploadAnon { get; set; }
    public bool DisableImageUploadAuth { get; set; }
    public bool DisableGridUploadAuth { get; set; }
}