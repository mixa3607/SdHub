using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace SdHub.Services.Tokens;

public class UserPasswordService : IUserPasswordService
{
    private const byte LastHasherVer = 1;
    private const int MinPasswordLength = 6;

    public bool CheckPasswordRequirements(string? password)
    {
        if (password == null)
            return false;
        if (password.Length < MinPasswordLength)
            return false;
        if (password.Contains(' '))
            return false;

        return true;
    }

    public bool Validate(string? password, string? passwordHash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(passwordHash))
            return false;
        var parts = passwordHash.Split('$');
        if (parts.Length != 3)
            return false;
        if (parts[0] != "1")
            return false;

        return CreatePasswordHashV1(password, parts[1]) == passwordHash;
    }

    public string CreatePasswordHash(string password)
    {
        return CreatePasswordHashV1(password);
    }

    private string CreatePasswordHashV1(string password, string? salt = null)
    {
        var saltBytes = salt == null
            ? RandomNumberGenerator.GetBytes(128 / 8)
            : Convert.FromBase64String(salt);
        var hashBytes = KeyDerivation.Pbkdf2(
            password: password!,
            salt: saltBytes,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8);
        var saltB64 = Convert.ToBase64String(saltBytes);
        var hashB64 = Convert.ToBase64String(hashBytes);

        var result = $"{LastHasherVer}${saltB64}${hashB64}";
        return result;
    }

}