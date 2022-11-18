namespace SdHub.Models.Image;

public class EditImageRequest
{
    public string? ManageToken { get; set; }

    public EditImageModel? Image { get; set; }
}