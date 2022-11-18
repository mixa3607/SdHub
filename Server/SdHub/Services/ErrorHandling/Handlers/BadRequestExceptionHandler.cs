using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SdHub.Services.ErrorHandling.Exceptions;

namespace SdHub.Services.ErrorHandling.Handlers;

public class BadRequestExceptionHandler : IServerExceptionHandler
{
    public Task<ServerErrorResponse?> HandleAsync(Exception ex, HttpContext ctx)
    {
        var result = (ServerErrorResponse?)null;
        if (ex is BadHttpRequestModelStateException ex1)
        {
            result = new ServerErrorResponse()
            {
                MoreData = "",
                Title = "Ошибка в данных",
                Message = ex.Message,
                InnerException = "",
                Guid = Guid.NewGuid(),
                ModelState = ex1.ModelErrors.ToDictionary(x => x.Key, x => (IReadOnlyList<string>)x.Value),
                StatusCode = ex1.StatusCode == 0 ? HttpStatusCode.BadRequest : (HttpStatusCode)ex1.StatusCode,
            };
        }

        return Task.FromResult(result);
    }
}