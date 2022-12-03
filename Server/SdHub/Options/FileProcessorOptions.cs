using System.ComponentModel.DataAnnotations;

namespace SdHub.Options;

/// <summary>
/// Files processings options
/// </summary>
public class FileProcessorOptions
{
    /// <summary>
    /// Directory for temp files
    /// </summary>
    [Required]
    public string? CacheDir { get; set; } = "./cache/upload";

    /// <summary>
    /// Don't delete cache
    /// </summary>
    public bool PreserveCache { get; set; }
}