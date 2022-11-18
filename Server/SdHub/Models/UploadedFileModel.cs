namespace SdHub.Models;

public class UploadedFileModel
{
    public bool Uploaded { get; set; }
    public string? Reason { get; set; }
    public string? ManageToken { get; set; }
    public ImageModel? Image { get; set; }
}