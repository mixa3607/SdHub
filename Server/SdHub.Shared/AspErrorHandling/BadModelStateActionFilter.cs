using Microsoft.AspNetCore.Mvc.Filters;
using SdHub.Shared.AspErrorHandling.ModelState;

namespace SdHub.Shared.AspErrorHandling;

public class BadModelStateActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        context.ModelState.ThrowIfNotValid();
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        //nothing
    }
}