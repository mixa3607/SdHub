using System.ComponentModel.DataAnnotations;

namespace SdHub.Shared.EntityFramework;

/// <summary>
/// pg db options
/// </summary>
public class PgDatabaseOptions
{
    /// <summary>
    /// Connection string
    /// </summary>
    [Required]
    public string ConnectionString { get; set; } = "";
}