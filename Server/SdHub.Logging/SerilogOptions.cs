using Serilog.Events;

namespace SdHub.Logging;

public class SerilogOptions
{
    public const string SectionName = "Serilog";
    public List<Uri> ElasticUris { get; set; } = new();
    public bool DisableElastic { get; set; }
    public string? ElasticIndexPrefix { get; set; }
    public SerilogOptionsLevelPresetType LevelPreset { get; set; } = SerilogOptionsLevelPresetType.Prod;
    public bool EnableRequestLogging { get; set; }
    public LogEventLevel RequestLogMessageLevel { get; set; } = LogEventLevel.Information;
}