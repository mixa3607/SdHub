namespace SdHub.Models.User;

public class ChangePasswordRequest
{
    public string? Password { get; set; }
    public string? NewPassword { get; set; }
}