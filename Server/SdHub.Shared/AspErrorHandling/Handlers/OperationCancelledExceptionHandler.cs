using System.Net;
using Microsoft.AspNetCore.Http;

namespace SdHub.Shared.AspErrorHandling.Handlers;

public class OperationCancelledExceptionHandler : IServerExceptionHandler
{
    public int Order => 100;
    public Task<ServerErrorResponse?> HandleAsync(Exception ex, HttpContext ctx)
    {
        var result = (ServerErrorResponse?)null;
        if (SearchCancelledEx(ex))
        {
            result = new ServerErrorResponse()
            {
                MoreData = "",
                Title = "cancelled",
                Message = ex.Message,
                InnerException = "",
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
