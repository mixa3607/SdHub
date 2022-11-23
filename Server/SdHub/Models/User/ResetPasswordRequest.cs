namespace SdHub.Models.User;

public class ResetPasswordRequest
{
    public string? Login { get; set; }
    public string? Code { get; set; }
    public string? NewPassword { get; set; }
}