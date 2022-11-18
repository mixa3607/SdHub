using Microsoft.AspNetCore.Mvc.ModelBinding;
using SdHub.Services.ErrorHandling.Exceptions;

namespace SdHub.Extensions;

public static class ModelStateDictionaryExtensions
{
    public static void ThrowIfNotValid(this ModelStateDictionary modelState)
    {
        if (!modelState.IsValid)
        {
            throw new BadHttpRequestModelStateException(modelState);
        }
    }
    public static void Throw(this ModelStateDictionary modelState)
    {
        throw new BadHttpRequestModelStateException(modelState);
    }

    public static ModelStateDictionary AddError(this ModelStateDictionary modelState, string error, params object[] args)
    {
        modelState.AddModelError("", ModelStateErrorHelper.Build(error, args));
        return modelState;
    }

    public static ModelStateDictionary AddError(this ModelStateDictionary modelState, string property, string error, params object[] args)
    {
        modelState.AddModelError(property, ModelStateErrorHelper.Build(error, args));
        return modelState;
    }

    public static ModelStateBuilder ToBuilder(this ModelStateDictionary modelState)
    {
        return new ModelStateBuilder(modelState);
    }
}