using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace SdHub.Shared.AspValidation;

public class AllRequiredValidatorsRegisteredService
{
    private readonly IServiceProvider _services;

    public AllRequiredValidatorsRegisteredService(IServiceProvider services)
    {
        _services = services;
    }

    public IReadOnlyList<ControllerActionValidationResult> CheckControllersParams(params Assembly[] skipAssemblies)
    {
        var actionDescriptors = _services.GetRequiredService<IActionDescriptorCollectionProvider>().ActionDescriptors;
        var controllerDescriptors = actionDescriptors.Items
            .OfType<ControllerActionDescriptor>()
            .ToArray();

        var results = new List<ControllerActionValidationResult>();
        foreach (var descriptor in controllerDescriptors)
        {
            var paramResults = new List<ControllerActionValidationResult.ArgumentValidationInfo>();

            var actionParams = descriptor.Parameters
                .Where(x => x.BindingInfo?.BindingSource?.IsFromRequest == true)
                .Cast<ControllerParameterDescriptor>()
                .ToArray();
            foreach (var param in actionParams)
            {
                var info = new ControllerActionValidationResult.ArgumentValidationInfo() { ParameterInfo = param.ParameterInfo, };
                paramResults.Add(info);
                var skipAttr = param.ParameterInfo.GetCustomAttribute<SkipValidatorsCheckAttribute>();
                if (skipAttr != null)
                {
                    info.SkipType = ControllerActionValidationResult.ValidationSkipType.Attribute;
                    info.SkipReason = skipAttr.Reason;
                    continue;
                }

                var type = Nullable.GetUnderlyingType(param.ParameterType) ?? param.ParameterType;
                if (skipAssemblies.Any(x => x == type.Assembly))
                {
                    info.SkipType = ControllerActionValidationResult.ValidationSkipType.Assembly;
                    info.SkipReason = "Assembly skip";
                    continue;
                }

                var validatorRequired = (type.IsClass || type.IsValueType) &&
                                        !type.IsPrimitive &&
                                        !type.Namespace!.StartsWith("System.");
                if (!validatorRequired)
                {
                    info.SkipType = ControllerActionValidationResult.ValidationSkipType.MemberType;
                    info.SkipReason = $"Type {type.Name} always skip";
                    continue;
                }

                var validatorType = typeof(IValidator<>).MakeGenericType(type);
                var validator = _services.GetService(validatorType);
                info.ValidatorFound = validator != null;
            }

            results.Add(new ControllerActionValidationResult()
            {
                Arguments = paramResults,
                ControllerName = descriptor.ControllerName,
                ActionName = descriptor.ActionName,
            });
        }

        if (results
            .SelectMany(x => x.Arguments)
            .Any(x => x is { SkipType: ControllerActionValidationResult.ValidationSkipType.No, ValidatorFound: false }))
        {
            throw new HaveMissedValidatorsException(results);
        }

        return results;
    }
}
