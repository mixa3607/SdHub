using System.Net;
using Microsoft.AspNetCore.Http;
using SdHub.Shared.AspErrorHandling.Exceptions;

namespace SdHub.Shared.AspErrorHandling.Handlers;

public class ServerExceptionHandler : IServerExceptionHandler
{
    public int Order => 2;
    public Task<ServerErrorResponse?> HandleAsync(Exception ex, HttpContext ctx)
    {
        var result = (ServerErrorResponse?)null;
        if (ex is ServerException ex1)
        {
            result = new ServerErrorResponse()
            {
                MoreData = ex1?.MoreData,
                Title = ex1?.Title ?? "Server error",
                Message = ex1?.Message,
                InnerException = ex1?.ToString(),
                StatusCode = HttpStatusCode.BadRequest,
            };
        }

        return Task.FromResult(result);
    }
}
