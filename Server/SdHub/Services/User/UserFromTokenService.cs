using System;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using SdHub.Models;
using SdHub.Constants;

namespace SdHub.Services.User;

public class UserFromTokenService : IUserFromTokenService
{
    private readonly IHttpContextAccessor _contextAccessor;
    private UserFromToken? _cached;

    public UserFromTokenService(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public UserModel? Get() =>
        GetTokenUser()?.User;

    public UserModel? Get(string jwt, TokenValidationParameters validationParams) =>
        GetTokenUser(jwt, validationParams)?.User;

    public UserModel? Get(ClaimsPrincipal? claims) =>
        GetTokenUser(claims)?.User;

    public UserFromToken? GetTokenUser()
    {
        if (_contextAccessor.HttpContext?.User.Identity?.IsAuthenticated != true)
            return null;
        var user = _contextAccessor.HttpContext?.User;
        if (user == null)
            return null;
        if (_cached != null)
            return _cached;

        var model = GetTokenUser(user);
        if (model != null)
        {
            //я честно хз почему именно "access_token" и откуда растут ноги
            model.JwtToken = _contextAccessor.HttpContext?.GetTokenAsync("access_token")
                .Result;
        }

        _cached = model;
        return model;
    }

    public UserFromToken? GetTokenUser(string jwt, TokenValidationParameters validationParams)
    {
        var handler = new JwtSecurityTokenHandler();
        var user = handler.ValidateToken(jwt, validationParams, out var secToken);
        //var token = new JwtSecurityToken(jwt);
        //var user = new ClaimsPrincipal(new ClaimsIdentity(token.Claims));

        var model = GetTokenUser(user);
        if (model != null)
        {
            model.JwtToken = jwt;
        }

        return model;
    }

    public UserFromToken? GetTokenUser(ClaimsPrincipal? claims)
    {
        var user = claims;
        if (user == null)
            return null;

        var model = new UserFromToken()
        {
            Roles = user.FindAll(ClaimTypes.Role).Select(x => x.Value).ToArray(),
            Email = user.FindFirst(ClaimTypes.Email)?.Value,
            Phone = user.FindFirst(ClaimTypes.MobilePhone)?.Value,
            Login = user.FindFirst(CustomClaimTypes.Login)?.Value,
            Guid = Guid.Parse(user.FindFirst(CustomClaimTypes.UserGuid)!.Value),
        };

        var userClaim = user.FindFirst(CustomClaimTypes.User)!;
        model.User = JsonConvert.DeserializeObject<UserModel>(userClaim.Value);

        return model;
    }
}