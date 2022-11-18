using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SdHub.Services.ErrorHandling.Exceptions;

namespace SdHub.Services.ErrorHandling.Handlers;

public class ServerExceptionHandler : IServerExceptionHandler
{
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
                Guid = Guid.NewGuid(),
                StatusCode = HttpStatusCode.BadRequest,
            };
        }

        return Task.FromResult(result);
    }
}