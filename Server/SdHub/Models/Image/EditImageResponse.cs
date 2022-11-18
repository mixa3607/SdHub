namespace SdHub.Models.Image;

public class EditImageResponse
{
    public ImageModel? Image { get; set; }
    public bool Success { get; set; }
    public string? Reason { get; set; }
}