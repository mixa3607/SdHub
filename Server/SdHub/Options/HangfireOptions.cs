using System.ComponentModel.DataAnnotations;

namespace SdHub.Options;

/// <summary>
/// Hangfire scheduler options. GUI on /hgf
/// </summary>
public class HangfireOptions
{
    public HangfireStorageType StorageType { get; set; }
    /// <summary>
    /// Postgres connection string
    /// </summary>
    [Required]
    public string? PgConnectionString { get; set; }

    /// <summary>
    /// Postgres schema
    /// </summary>
    [Required]
    public string? PgSchema { get; set; } = "public";

    /// <summary>
    /// Redis connection string
    /// </summary>
    [Required]
    public string? RedisConnectionString { get; set; }

    /// <summary>
    /// Redis tables prefix
    /// </summary>
    [Required]
    public string? RedisPrefix { get; set; }

    /// <summary>
    /// Run hangfire worker on backend
    /// </summary>
    public bool RunServer { get; set; } = true;

    /// <summary>
    /// Worker name
    /// </summary>
    [Required]
    public string? ServerName { get; set; } = "bakend";

    /// <summary>
    /// Special queue for this worker
    /// </summary>
    [Required]
    public string? ServerQueue { get; set; } = "backend";
}

public enum HangfireStorageType : byte
{
    Redis,
    Postgres,
    InMemory,
}