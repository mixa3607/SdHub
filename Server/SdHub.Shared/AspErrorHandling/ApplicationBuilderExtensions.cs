using Microsoft.AspNetCore.Builder;
using SdHub.Shared.AspErrorHandling.Middleware;

namespace SdHub.Shared.AspErrorHandling;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseRbErrorsHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ErrorHandlingMiddleware>();
    }
}