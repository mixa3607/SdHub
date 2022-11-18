using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SdHub.Services.ErrorHandling.Handlers;

public class OperationCancelledExceptionHandler : IServerExceptionHandler
{
    public Task<ServerErrorResponse?> HandleAsync(Exception ex, HttpContext ctx)
    {
        var result = (ServerErrorResponse?)null;
        if (SearchCancelledEx(ex))
        {
            result = new ServerErrorResponse()
            {
                MoreData = "",
                Title = "отменено",
                Message = ex.Message,
                InnerException = "",
                Guid = Guid.NewGuid(),
                StatusCode = (HttpStatusCode)777
            };
        }

        return Task.FromResult(result);
    }

    private bool SearchCancelledEx(Exception ex)
    {
        if (ex is OperationCanceledException)
        {
            return true;
        }
        else if (ex is AggregateException aggEx)
        {
            return aggEx.InnerExceptions.Any(SearchCancelledEx);
        }
        else if (ex.InnerException != null)
        {
            return SearchCancelledEx(ex.InnerException);
        }

        return false;
    }
}