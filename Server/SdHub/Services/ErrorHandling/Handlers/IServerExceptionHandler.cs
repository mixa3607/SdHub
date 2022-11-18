using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SdHub.Services.ErrorHandling.Handlers;

public interface IServerExceptionHandler
{
    Task<ServerErrorResponse?> HandleAsync(Exception ex, HttpContext ctx);
}