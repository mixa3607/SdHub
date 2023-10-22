using Microsoft.AspNetCore.Mvc.ModelBinding;
using SdHub.Shared.AspErrorHandling.Exceptions;

namespace SdHub.Shared.AspErrorHandling.ModelState;

public class ModelStateBuilder
{
    private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

    public ModelStateBuilder()
    {
    }

    public ModelStateBuilder(ModelStateDictionary dict)
    {
        foreach (var dictValue in dict)
        {
            foreach (var valueError in dictValue.Value.Errors)
            {
                AddError(dictValue.Key, valueError);
            }
        }
    }

    public ModelStateBuilder AddError(string property, string error, params object?[] args)
    {
        GetErrsListForProp(property).Add(ModelStateErrorHelper.Build(error, args));
        return this;
    }

    public ModelStateBuilder AddError(string error, params object?[] args)
    {
        GetErrsListForProp("").Add(ModelStateErrorHelper.Build(error, args));
        return this;
    }

    public IReadOnlyDictionary<string, IReadOnlyList<string>> Build()
    {
        return _errors.ToDictionary(x => x.Key, x => (IReadOnlyList<string>)x.Value);
    }

    /// <summary>
    /// Кидает исключение если есть ошибки
    /// </summary>
    /// <param name="statusCode"></param>
    /// <exception cref="BadHttpRequestModelStateException"></exception>
    public void ThrowIfNotValid(int statusCode = 400)
    {
        if (!IsValid())
        {
            throw new BadHttpRequestModelStateException(_errors, statusCode);
        }
    }

    public bool IsValid()
    {
        return !_errors.Any(x => x.Value.Count > 0);
    }

    private List<string> GetErrsListForProp(string property)
    {
        if (!_errors.TryGetValue(property, out var errs))
        {
            errs = new List<string>();
            _errors[property] = errs;
        }

        return errs;
    }
}