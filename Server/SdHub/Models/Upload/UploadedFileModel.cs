using SdHub.Models.Image;

namespace SdHub.Models.Upload;

public class UploadedFileModel
{
    public bool Uploaded { get; set; }
    public string? Reason { get; set; }
    public ImageModel? Image { get; set; }
}