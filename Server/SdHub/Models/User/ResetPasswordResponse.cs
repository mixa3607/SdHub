namespace SdHub.Models.User;

public class ResetPasswordResponse
{
    public required UserModel User { get; set; }
    public required bool Success { get; set; }
}