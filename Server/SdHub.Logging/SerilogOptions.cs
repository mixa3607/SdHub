using Serilog.Events;
using System.ComponentModel.DataAnnotations;

namespace SdHub.Logging;

/// <summary>
/// Options for logging to ES. Used serilogs-sink-{assemblyName}-{0:yyyy.MM.dd} index
/// </summary>
public class SerilogOptions
{
    /// <summary>
    /// Disable logging to elastic search
    /// </summary>
    public bool DisableElastic { get; set; } = true;

    /// <summary>
    /// Urls to ES cluster
    /// </summary>
    [Required]
    public List<Uri> ElasticUris { get; set; } = new();

    /// <summary>
    /// Append {prefix}- to index
    /// </summary>
    public string? ElasticIndexPrefix { get; set; }

    /// <summary>
    /// Predefined levels for logging <see cref="CustomSerilogLoggingExtensions.AddCustomSerilog"/>
    /// </summary>
    public SerilogOptionsLevelPresetType LevelPreset { get; set; } = SerilogOptionsLevelPresetType.Prod;

    /// <summary>
    /// Enable request logging
    /// </summary>
    public bool EnableRequestLogging { get; set; } = true;

    /// <summary>
    /// Request message log level
    /// </summary>
    public LogEventLevel RequestLogMessageLevel { get; set; } = LogEventLevel.Information;
}