namespace SdHub.Options;

public class HangfireOptions
{
    public string? DatabaseConnectionString { get; set; }
    public string? DatabaseSchema { get; set; }
    public bool RunServer { get; set; }
    public string? ServerName { get; set; }
}