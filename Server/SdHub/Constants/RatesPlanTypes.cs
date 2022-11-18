using System;
using System.Linq;
using System.Reflection;

namespace SdHub.Constants;

public static class RatesPlanTypes
{
    public const string AdminPlan = nameof(AdminPlan);
    public const string RegUserPlan = nameof(RegUserPlan);
    public const string AnonUserPlan = nameof(AnonUserPlan);

    private static string[]? _all;

    public static string? Parse(string? name)
    {
        _all ??= typeof(RatesPlanTypes)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(x => x.IsLiteral && x.FieldType == typeof(string))
            .Select(x => (string)x.GetRawConstantValue()!)
            .ToArray();

        return name == null
            ? null
            : _all.FirstOrDefault(x => x.Equals(name, StringComparison.InvariantCultureIgnoreCase));
    }
}