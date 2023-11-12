namespace SdHub.Models.User;

public class ConfirmEmailResponse
{
    public required UserModel User { get; set; }
    public required bool Success { get; set; }
}