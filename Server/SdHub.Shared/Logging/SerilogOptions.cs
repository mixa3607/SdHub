using Serilog.Events;

namespace SdHub.Shared.Logging;

/// <summary>
/// Options for logging to serilog
/// </summary>
public class SerilogOptions
{
    /// <summary>
    /// Enable request logging
    /// </summary>
    public bool EnableRequestLogging { get; set; } = true;

    /// <summary>
    /// Add request body to log? *not recommend
    /// </summary>
    public bool EnableRequestBodyLogging { get; set; } = false;

    /// <summary>
    /// Request message log level
    /// </summary>
    public LogEventLevel RequestLogMessageLevel { get; set; } = LogEventLevel.Information;
}
