namespace SdHub.Models.User;

public class ResetPasswordRequest
{
    public required string Login { get; set; }
    public required string Code { get; set; }
    public required string NewPassword { get; set; }
}