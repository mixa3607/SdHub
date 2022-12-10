using SdHub.Models.Grid;

namespace SdHub.Models.Upload;

public class UploadGridResponse
{
    public bool Uploaded { get; set; }
    public string? Reason { get; set; }
    public GridModel? Grid { get; set; }
}