using System.ComponentModel.DataAnnotations;

namespace SdHub.Options;

/// <summary>
/// Main db options
/// </summary>
public class DatabaseOptions
{
    /// <summary>
    /// Connection string
    /// </summary>
    [Required]
    public string ConnectionString { get; set; } = "";
}