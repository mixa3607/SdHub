using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SdHub.Constants;
using SdHub.Database;
using SdHub.Database.Entities.User;

namespace SdHub.ApiTokenAuth;

public class ApiTokenAuthHandler : AuthenticationHandler<ApiTokenAuthSchemeOptions>
{
    private readonly SdHubDbContext _db;

    public ApiTokenAuthHandler(
        IOptionsMonitor<ApiTokenAuthSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock, SdHubDbContext db)
        : base(options, logger, encoder, clock)
    {
        _db = db;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authHeader = Request.Headers.Authorization.ToString();
        if (authHeader != null && authHeader.StartsWith(Scheme.Name, StringComparison.OrdinalIgnoreCase))
        {
            var token = authHeader[Scheme.Name.Length..].Trim();
            if (token == "test")
            {
                var user = await _db.Users.FirstAsync(x => x.Roles.Contains(UserRoleTypes.Admin));
                return AuthenticateResult.Success(CreateTicket(user));
            }
        }

        Response.StatusCode = 401;
        Response.Headers.Add("WWW-Authenticate", Scheme.Name);
        return AuthenticateResult.Fail("Invalid Authorization Header");
    }

    private AuthenticationTicket CreateTicket(UserEntity user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Guid.ToString()),
            new Claim(ClaimTypes.Name, user.Login ?? ""),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return ticket;
    }
}