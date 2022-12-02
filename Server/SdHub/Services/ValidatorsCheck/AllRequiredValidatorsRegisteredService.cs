using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SdHub.Services.ValidatorsCheck;

public class AllRequiredValidatorsRegisteredService
{
    private readonly IServiceProvider _services;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<AllRequiredValidatorsRegisteredService> _logger;

    public AllRequiredValidatorsRegisteredService(IServiceProvider services, IHostEnvironment environment,
        ILogger<AllRequiredValidatorsRegisteredService> logger)
    {
        _services = services;
        _environment = environment;
        _logger = logger;
    }

    public void Check()
    {
        if (!_environment.IsDevelopment())
        {
            _logger.LogWarning("Skip validators check in non Development env");
            return;
        }

        var missingValidators = new List<MissingValidatorError>();

        var assemblies = new Assembly[] { GetType().Assembly };
        var controllers = assemblies.SelectMany(x => x.GetTypes()).Where(x => x.BaseType == typeof(ControllerBase));
        foreach (var controller in controllers)
        {
            var methods = controller
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(x => x.GetCustomAttribute<HttpGetAttribute>() != null ||
                            x.GetCustomAttribute<HttpPostAttribute>() != null ||
                            x.GetCustomAttribute<HttpPutAttribute>() != null ||
                            x.GetCustomAttribute<HttpPatchAttribute>() != null)
                .ToArray();
            var args = methods
                .SelectMany(x => x.GetParameters())
                .Where(x => x.GetCustomAttribute<SkipValidatorsCheckAttribute>() == null)
                .Select(x => Nullable.GetUnderlyingType(x.ParameterType) ?? x.ParameterType)
                .Where(x => (x.IsClass || x.IsValueType) &&
                            !x.IsPrimitive &&
                            !x.Namespace!.StartsWith("System."))
                .Distinct()
                .ToArray();
            foreach (var arg in args)
            {
                var validatorType = typeof(IValidator<>).MakeGenericType(arg);
                try
                {
                    _services.GetRequiredService(validatorType);
                }
                catch (Exception)
                {
                    missingValidators.Add(new MissingValidatorError(controller, arg));
                }
            }
        }

        if (missingValidators.Count > 0)
        {
            throw new HaveMissedValidatorsException(missingValidators);
        }
    }
}