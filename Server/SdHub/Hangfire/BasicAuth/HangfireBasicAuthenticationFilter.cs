using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SdHub.Constants;

namespace SdHub.Hangfire.BasicAuth;

public class HangfireBasicAuthenticationFilter : IDashboardAsyncAuthorizationFilter
{
    private const string AuthenticationScheme = "Basic";
    private readonly bool _isReadOnlyView;

    public HangfireBasicAuthenticationFilter(bool isReadOnlyView)
    {
        _isReadOnlyView = isReadOnlyView;
    }

    public async Task<bool> AuthorizeAsync(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        var sp = httpContext.RequestServices;
        var logger = sp.GetRequiredService<ILogger<HangfireBasicAuthenticationFilter>>();
        var usersService = sp.GetRequiredService<IHangfireUsersService>();
        var ct = httpContext.RequestAborted;

        string username;
        string password;
        try
        {
            var authHeader = AuthenticationHeaderValue.Parse(httpContext.Request.Headers.Authorization);
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
            username = credentials[0];
            password = credentials[1];
        }
        catch (Exception e)
        {
            logger.LogError(e, "Cant parse basic auth header");
            logger.LogInformation("Request is missing Authorization Header");
            SetChallengeResponse(httpContext);
            return false;
        }

        var user = await usersService.GetUserByNameAsync(username, ct);
        var ro = user?.Roles.Contains(UserRoleTypes.HangfireRO) == true;
        if (user == null)
        {
            logger.LogInformation("User {user} not found", username);
            SetChallengeResponse(httpContext);
            return false;
        }
        else if (!usersService.IsPasswordCorrect(user, password))
        {
            logger.LogInformation("Password {pwd} not correct for user {user}", password, username);
            SetChallengeResponse(httpContext);
            return false;
        }

        var claims = user.Roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
        claims.Add(new Claim(ClaimTypes.Name, username));
        httpContext.User.AddIdentity(new ClaimsIdentity(claims, AuthenticationScheme));
        return true;
    }

    private void SetChallengeResponse(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = 401;
        httpContext.Response.Headers.Append("WWW-Authenticate", "Basic realm=\"Hangfire Dashboard\"");
    }
}