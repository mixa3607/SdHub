namespace SdHub.Services.FileProc.Metadata;

public class ImageMetadataTagValue
{
    public string? AsString { get; set; }
    public object? Object { get; set; }

    public string? Type { get; set; }
    public bool IsArray { get; set; }

    public override string ToString()
    {
        return AsString ?? "";
    }
}