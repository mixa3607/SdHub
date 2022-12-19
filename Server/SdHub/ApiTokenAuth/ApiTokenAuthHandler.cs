using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using SdHub.Constants;
using SdHub.Database;
using SdHub.Database.Entities.Users;
using SdHub.Models;
using SdHub.RequestFeatures;

namespace SdHub.ApiTokenAuth;

public class ApiTokenAuthHandler : AuthenticationHandler<ApiTokenAuthSchemeOptions>
{
    private readonly SdHubDbContext _db;
    private readonly IMapper _mapper;

    public ApiTokenAuthHandler(
        IOptionsMonitor<ApiTokenAuthSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock, SdHubDbContext db, IMapper mapper)
        : base(options, logger, encoder, clock)
    {
        _db = db;
        _mapper = mapper;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authHeader = Request.Headers.Authorization.ToString();
        if (authHeader != null && authHeader.StartsWith(Scheme.Name, StringComparison.OrdinalIgnoreCase))
        {
            var token = authHeader[Scheme.Name.Length..].Trim();
            var userToken = await _db.ApiTokens
                .Include(x => x.User)
                .Where(x => x.Token == token && x.User!.DeletedAt == null)
                .FirstOrDefaultAsync();
            if (userToken == null)
                return AuthenticateResult.Fail(new ApiTokenAuthException("token_not_found"));
            if (userToken.ExpiredAt == default)
                return AuthenticateResult.Fail(new ApiTokenAuthException("token_revoked"));
            if (userToken.ExpiredAt < DateTimeOffset.Now)
                return AuthenticateResult.Fail(new ApiTokenAuthException("token_expired", $"Token expired at {userToken.ExpiredAt}"));

            var userModel = _mapper.Map<UserModel>(userToken.User!);
            Context.Features.Set(new ApiTokenAuthFeature(userModel));
            return AuthenticateResult.Success(CreateTicket(userToken.User!));
        }

        Response.Headers.Add("WWW-Authenticate", Scheme.Name);
        return AuthenticateResult.Fail(new ApiTokenAuthException("invalid_header"));
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        var authResult = await HandleAuthenticateOnceSafeAsync();
        if (authResult.Succeeded)
            return;

        Response.StatusCode = 401;

        var eventContext = authResult.Failure as ApiTokenAuthException;
        if (eventContext == null)
            return;

        if (string.IsNullOrEmpty(eventContext.Error) &&
            string.IsNullOrEmpty(eventContext.ErrorDescription) &&
            string.IsNullOrEmpty(eventContext.ErrorUri))
        {
            Response.Headers.Append(HeaderNames.WWWAuthenticate, Options.Challenge);
        }
        else
        {
            // https://tools.ietf.org/html/rfc6750#section-3.1
            // WWW-Authenticate: ApiToken realm="example", error="invalid_token", error_description="The access token expired"
            var builder = new StringBuilder(Options.Challenge);
            if (Options.Challenge.IndexOf(' ') > 0)
            {
                // Only add a comma after the first param, if any
                builder.Append(',');
            }
            if (!string.IsNullOrEmpty(eventContext.Error))
            {
                builder.Append(" error=\"");
                builder.Append(eventContext.Error);
                builder.Append('\"');
            }
            if (!string.IsNullOrEmpty(eventContext.ErrorDescription))
            {
                if (!string.IsNullOrEmpty(eventContext.Error))
                {
                    builder.Append(',');
                }

                builder.Append(" error_description=\"");
                builder.Append(eventContext.ErrorDescription);
                builder.Append('\"');
            }
            if (!string.IsNullOrEmpty(eventContext.ErrorUri))
            {
                if (!string.IsNullOrEmpty(eventContext.Error) ||
                    !string.IsNullOrEmpty(eventContext.ErrorDescription))
                {
                    builder.Append(',');
                }

                builder.Append(" error_uri=\"");
                builder.Append(eventContext.ErrorUri);
                builder.Append('\"');
            }

            Response.Headers.Append(HeaderNames.WWWAuthenticate, builder.ToString());
        }
    }

    private AuthenticationTicket CreateTicket(UserEntity user)
    {
        var claims = new List<Claim>(5 + user.Roles.Count)
        {
            new Claim(ClaimTypes.NameIdentifier, user.Guid.ToString()),
            new Claim(ClaimTypes.Name, user.Login ?? ""),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(CustomClaimTypes.UserGuid, user.Guid.ToString()),
            new Claim(ClaimTypes.AuthenticationMethod, ApiTokenDefaults.AuthenticationScheme),
        };
        claims.AddRange(user.Roles.Select(x => new Claim(ClaimTypes.Role, x)));

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return ticket;
    }
}