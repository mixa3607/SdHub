using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using SdHub.Options;
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
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = options.Jwt.LogPii;
                o.SaveToken = true;
                o.RequireHttpsMetadata = options.EnableHttpsRedirections;
                o.TokenValidationParameters = options.MapToTokenValidationParameters();
            });
        return services;
    }
}