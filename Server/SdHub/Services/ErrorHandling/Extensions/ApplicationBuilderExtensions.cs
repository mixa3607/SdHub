using Microsoft.AspNetCore.Builder;

namespace SdHub.Services.ErrorHandling.Extensions;

public static class ApplicationBuilderExtensions
{

    public static IApplicationBuilder UseCustomErrorHandling(this IApplicationBuilder app)
    {
        app.UseMiddleware<ErrorHandlingMiddleware>();
        return app;
    }
}