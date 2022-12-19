using Microsoft.AspNetCore.Authentication.JwtBearer;
using SdHub.ApiTokenAuth;

namespace SdHub.Constants;

public static class AuthConst
{
    public const string JwtAndApiScheme =
        $"{ApiTokenDefaults.AuthenticationScheme},{JwtBearerDefaults.AuthenticationScheme}";
}