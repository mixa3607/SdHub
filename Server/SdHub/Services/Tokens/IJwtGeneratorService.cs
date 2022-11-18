using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using SdHub.Models;

namespace SdHub.Services.Tokens;

public interface IJwtGeneratorService
{
    Task<string> GenerateAsync(UserModel user, TimeSpan? expTime = null,
        CancellationToken ct = default);

    JsonWebKey GenerateJwk();
}