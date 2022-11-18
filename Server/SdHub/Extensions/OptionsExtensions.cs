using System;
using Microsoft.IdentityModel.Tokens;
using SdHub.Options;

namespace SdHub.Extensions;

public static class OptionsExtensions
{
    public static TokenValidationParameters MapToTokenValidationParameters(this WebSecurityOptions options)
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidIssuers = options.Jwt.Issuers,
            ValidateAudience = false,
            ValidAudiences = options.Jwt.Audiences,
            ValidateLifetime = options.Jwt.ValidateLifetime,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = options.Jwt.PublicKey,
            ClockSkew = TimeSpan.FromSeconds(10)
        };
    }
}