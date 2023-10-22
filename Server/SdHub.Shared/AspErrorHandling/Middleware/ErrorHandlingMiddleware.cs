using System.Diagnostics;
using System.Net;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SdHub.Shared.AspErrorHandling.Handlers;

namespace SdHub.Shared.AspErrorHandling.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly RequestDelegate _next;
    private readonly IReadOnlyList<IServerExceptionHandler> _handlers;
    private readonly IJsonHelper _jsonHelper;

    public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger, RequestDelegate next,
        IEnumerable<IServerExceptionHandler> handlers, IJsonHelper jsonHelper)
    {
        _logger = logger;
        _next = next;
        _handlers = handlers.OrderBy(x => x.Order).ToArray();
        _jsonHelper = jsonHelper;
    }

    [DebuggerHidden]
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var result = (ServerErrorResponse?)null;
            foreach (var handler in _handlers)
            {
                result = await handler.HandleAsync(ex, context);
                if (result != null)
                    break;
            }

            result ??= HandleUnknownException(ex);
            WriteResponse(context, result);
        }
    }

    private ServerErrorResponse HandleUnknownException(Exception ex)
    {
        _logger.LogError(ex, "Handle unknown exception");
        return new ServerErrorResponse()
        {
            Title = "Unknown error",
            Message = ex.Message,
            InnerException = ex.ToString(),
            StatusCode = HttpStatusCode.InternalServerError
        };
    }

    private void WriteResponse(HttpContext context, ServerErrorResponse err)
    {
        try
        {
            err.TraceIdentifier = context.TraceIdentifier;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)err.StatusCode;

            var respStream = context.Response.BodyWriter.AsStream();
            using var textWriter = new StreamWriter(respStream);
            _jsonHelper.Serialize(err).WriteTo(textWriter, HtmlEncoder.Default);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Err when set err to resp");
        }
    }
}
