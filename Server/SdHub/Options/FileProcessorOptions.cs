namespace SdHub.Options;

public class FileProcessorOptions
{
    public string? CacheDir { get; set; } = "./cache/upload";
    public bool PreserveCache { get; set; }
}