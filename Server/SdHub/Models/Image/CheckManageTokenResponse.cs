namespace SdHub.Models.Image;

public class CheckManageTokenResponse
{
    public string? ShortToken { get; set; }
    public string? ManageToken { get; set; }
    public bool IsValid { get; set; }
}