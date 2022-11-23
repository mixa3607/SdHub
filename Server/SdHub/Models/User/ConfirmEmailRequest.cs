namespace SdHub.Models.User;

public class ConfirmEmailRequest
{
    public string? Code { get; set; }
    public string? Login { get; set; }
}