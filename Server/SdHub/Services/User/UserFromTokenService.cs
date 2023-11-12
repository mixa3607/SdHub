using System;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using SdHub.ApiTokenAuth;
using SdHub.Models;
using SdHub.Constants;
using SdHub.RequestFeatures;

namespace SdHub.Services.User;

public class UserFromTokenService : IUserFromTokenService
{
    private readonly IHttpContextAccessor _contextAccessor;
    private UserFromToken? _cachedJwt;
    private ApiTokenAuthFeature? _cachedApi;

    public UserFromTokenService(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public UserModel? Get() =>
        GetTokenUser()?.User ?? GetApiUser();

    public UserModel? GetApiUser()
    {
        if (_cachedApi != null)
            return _cachedApi.User;

        var user = _contextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
            return null;
        if (user.FindFirst(ClaimTypes.AuthenticationMethod)?.Value != ApiTokenDefaults.AuthenticationScheme)
            return null;

        _cachedApi = _contextAccessor.HttpContext!.Features.Get<ApiTokenAuthFeature>();
        return _cachedApi!.User;
    }

    public UserFromToken? GetTokenUser()
    {
        if (_cachedJwt != null)
            return _cachedJwt;
        var user = _contextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
            return null;

        var model = GetTokenUser(user);
        _cachedJwt = model;
        return model;
    }

    public UserFromToken? GetTokenUser(ClaimsPrincipal? claims)
    {
        var user = claims;
        if (user == null)
            return null;
        if (user.FindFirst(ClaimTypes.AuthenticationMethod)?.Value != JwtBearerDefaults.AuthenticationScheme)
            return null;

        var model = new UserFromToken()
        {
            Roles = user.FindAll(ClaimTypes.Role).Select(x => x.Value).ToArray(),
            Email = user.FindFirst(ClaimTypes.Email)?.Value,
            Login = user.FindFirst(CustomClaimTypes.Login)?.Value,
            Guid = Guid.Parse(user.FindFirst(CustomClaimTypes.UserGuid)!.Value),
        };

        var userClaim = user.FindFirst(CustomClaimTypes.User)!;
        model.User = JsonConvert.DeserializeObject<UserModel>(userClaim.Value);

        return model;
    }
}