using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using SdHub.ApiTokenAuth;
using SdHub.Options;
using SdHub.RequestFeatures;
using SdHub.Services.Tokens;

namespace SdHub.Extensions;

public static class SecurityStartupExtensions
{
    public static IServiceCollection AddCustomCors(this IServiceCollection services, WebSecurityOptions options)
    {
        services.AddCors(o =>
        {
            o.AddDefaultPolicy(p =>
            {
                p.WithOrigins(options.Cors.AllowedHosts)
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithExposedHeaders("*");
            });
        });
        return services;
    }

    public static IServiceCollection AddCustomSecurity(this IServiceCollection services, WebSecurityOptions options)
    {
        services
            .AddSingleton<IJwtGeneratorService, JwtGeneratorService>()
            .AddSingleton<IUserPasswordService, UserPasswordService>()
            .AddScoped<IRefreshTokenService, RefreshTokenService>()
            ;
        services
            .AddAuthentication(o => { o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
            .AddJwtBearer(o =>
            {
                Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = options.Jwt.LogPii;
                o.SaveToken = true;
                o.RequireHttpsMetadata = options.EnableHttpsRedirections;
                o.TokenValidationParameters = options.MapToTokenValidationParameters();
                o.Events = new JwtBearerEvents();
                o.Events.OnTokenValidated = ctx =>
                {
                    var identity = (ctx.Principal!.Identity! as ClaimsIdentity)!;
                    identity.AddClaim(
                        new Claim(ClaimTypes.AuthenticationMethod, JwtBearerDefaults.AuthenticationScheme));
                    return Task.CompletedTask;
                };
                o.Events.OnAuthenticationFailed = ctx =>
                {
                    ctx.HttpContext.Features.Set(new JwtAuthFailedFeature(ctx.Exception));
                    return Task.CompletedTask;
                };
            })
            .AddScheme<ApiTokenAuthSchemeOptions, ApiTokenAuthHandler>(ApiTokenDefaults.AuthenticationScheme, o => { });
        return services;
    }
}