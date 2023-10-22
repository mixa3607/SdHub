using System.Net;
using Microsoft.AspNetCore.Http;
using SdHub.Shared.AspErrorHandling.Exceptions;

namespace SdHub.Shared.AspErrorHandling.Handlers;

public class BadRequestExceptionHandler : IServerExceptionHandler
{
    public int Order => 1;

    public Task<ServerErrorResponse?> HandleAsync(Exception ex, HttpContext ctx)
    {
        var result = (ServerErrorResponse?)null;
        if (ex is BadHttpRequestModelStateException ex1)
        {
            result = new ServerErrorResponse()
            {
                MoreData = "",
                Title = "Bad request",
                Message = ex.Message,
                InnerException = "",
                ModelState = ex1.ModelErrors.ToDictionary(x => x.Key, x => (IReadOnlyList<string>)x.Value),
                StatusCode = ex1.StatusCode == 0 ? HttpStatusCode.BadRequest : (HttpStatusCode)ex1.StatusCode,
            };
        }

        return Task.FromResult(result);
    }
}
