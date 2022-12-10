namespace SdHub.Services.GridProc;


public class TilerOptions
{
    public string? SourceDir { get; set; }
    public string? LayersRoot { get; set; }
    public int XCount { get; set; }
    public int YCount { get; set; }
    public int MinLayer { get; set; }
}