using System.ComponentModel.DataAnnotations;

namespace SdHub.Options;

/// <summary>
/// Hangfire scheduler options. GUI on /hgf
/// </summary>
public class HangfireOptions
{
    /// <summary>
    /// Database connection string
    /// </summary>
    [Required]
    public string? DatabaseConnectionString { get; set; }

    /// <summary>
    /// Database schema
    /// </summary>
    [Required]
    public string? DatabaseSchema { get; set; } = "public";

    /// <summary>
    /// Run hangfire worker on backend
    /// </summary>
    public bool RunServer { get; set; } = true;

    /// <summary>
    /// Worker name
    /// </summary>
    [Required]
    public string? ServerName { get; set; } = "bakend";
}