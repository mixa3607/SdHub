using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using SdHub.Models;

namespace SdHub.Services.User;

public interface IUserFromTokenService
{
    UserModel? Get();
    //UserModel? Get(string jwt, TokenValidationParameters validationParams);
    //UserModel? Get(ClaimsPrincipal? claims);
    //UserFromToken? GetTokenUser();
    //UserFromToken? GetTokenUser(string jwt, TokenValidationParameters validationParams);
    //UserFromToken? GetTokenUser(ClaimsPrincipal? claims);
}