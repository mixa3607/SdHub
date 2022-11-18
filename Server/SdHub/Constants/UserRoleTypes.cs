using System;
using System.Linq;
using System.Reflection;

namespace SdHub.Constants;

public static class UserRoleTypes
{
    public const string User = nameof(User);
    public const string Admin = nameof(Admin);

    private static string[]? _allRoles;

    public static string? Parse(string? name)
    {
        _allRoles ??= typeof(UserRoleTypes)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(x => x.IsLiteral && x.FieldType == typeof(string))
            .Select(x => (string)x.GetRawConstantValue()!)
            .ToArray();

        return name == null
            ? null
            : _allRoles.FirstOrDefault(x => x.Equals(name, StringComparison.InvariantCultureIgnoreCase));
    }
}