namespace SdHub.Models.Image;

public class CanEditResponse
{
    public string? ShortToken { get; set; }
    public bool CanEdit { get; set; }
    public bool ManageTokenRequired { get; set; }
}