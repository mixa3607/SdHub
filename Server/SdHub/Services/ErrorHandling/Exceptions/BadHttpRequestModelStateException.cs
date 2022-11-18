using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SdHub.Services.ErrorHandling.Exceptions;

public class BadHttpRequestModelStateException : BadHttpRequestException
{
    public Dictionary<string, List<string>> ModelErrors { get; }

    public BadHttpRequestModelStateException(Dictionary<string, List<string>> modelErrors, int statusCode)
        : base("Model state errors", statusCode)
    {
        ModelErrors = modelErrors;
    }

    public BadHttpRequestModelStateException(Dictionary<string, List<string>> modelErrors) : base("Model state errors")
    {
        ModelErrors = modelErrors;
    }

    public BadHttpRequestModelStateException(ModelStateDictionary modelState, int statusCode)
        : base("Model state errors", statusCode)
    {
        ModelErrors = MapModelState(modelState);
    }

    public BadHttpRequestModelStateException(ModelStateDictionary modelState) : base("Model state errors")
    {
        ModelErrors = MapModelState(modelState);
    }


    private static Dictionary<string, List<string>> MapModelState(ModelStateDictionary modelState)
    {
        var result = new Dictionary<string, List<string>>();
        foreach (var (key, value) in modelState)
        {
            if (!result.TryGetValue(key, out var values))
            {
                values = new List<string>();
                result.Add(key, values);
            }

            values.AddRange(value.Errors.Select(x => x.ErrorMessage));
        }
        return result;
    }
}