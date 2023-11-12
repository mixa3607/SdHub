using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SdHub.Constants;
using SdHub.Models;
using SdHub.Options;

namespace SdHub.Services.Tokens;

public class JwtGeneratorService : IJwtGeneratorService
{
    private readonly IOptions<WebSecurityOptions> _options;
    private readonly JsonSerializerSettings _jsonSerializerSettings;

    public JwtGeneratorService(IOptions<WebSecurityOptions> options)
    {
        _options = options;
        _jsonSerializerSettings = new JsonSerializerSettings();
    }

    public Task<string> GenerateAsync(UserModel user, TimeSpan? expTime = null,
        CancellationToken ct = default)
    {
        var authClaims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(CustomClaimTypes.User, JsonConvert.SerializeObject(user)),
            new Claim(CustomClaimTypes.UserGuid, user.Guid.ToString())
        };
        //if (user.Email != null)
        //    authClaims.Add(new Claim(ClaimTypes.Email, user.Email));
        if (user.Roles != null)
        {
            foreach (var userRole in user.Roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }
        }

        var jwtOptions = _options.Value.Jwt;
        var jwtExpires = DateTime.UtcNow.Add(expTime ?? jwtOptions.JwtLifetime);

        var payload = new JwtPayload(jwtOptions.Issuer, null, authClaims, DateTime.UtcNow, jwtExpires, DateTime.UtcNow);

        var header = new JwtHeader(jwtOptions.SigningCredentials);
        var token = new JwtSecurityToken(header, payload);
        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }

    public JsonWebKey GenerateJwk()
    {
        return JsonWebKeyConverter.ConvertFromX509SecurityKey(_options.Value.Jwt.PublicKey);
    }
}