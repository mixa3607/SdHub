namespace SdHub.Models.User;

public class RegisterResponse
{
    public required UserModel User { get; set; }
    public required string SendToEmail { get; set; }
}