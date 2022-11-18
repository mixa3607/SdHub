namespace SdHub.Services.Tokens;

public interface IUserPasswordService
{
    bool CheckPasswordRequirements(string? password);

    string CreatePasswordHash(string password);
    bool Validate(string? password, string? passwordHash);
}