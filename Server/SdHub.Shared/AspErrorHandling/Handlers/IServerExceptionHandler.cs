using Microsoft.AspNetCore.Http;

namespace SdHub.Shared.AspErrorHandling.Handlers;

public interface IServerExceptionHandler
{
    int Order { get; }
    Task<ServerErrorResponse?> HandleAsync(Exception ex, HttpContext ctx);
}