using System.Reflection;

namespace SdHub.Shared.AspValidation;

public class ControllerActionValidationResult
{
    public required string ControllerName { get; set; }
    public required string ActionName { get; set; }
    public IReadOnlyCollection<ArgumentValidationInfo> Arguments { get; set; } = Array.Empty<ArgumentValidationInfo>();

    public override string ToString()
    {
        return $"{ControllerName}.{ActionName}({string.Join(", ", Arguments)});";
    }

    public class ArgumentValidationInfo
    {
        public ValidationSkipType SkipType { get; set; }
        public string? SkipReason { get; set; }
        public required ParameterInfo ParameterInfo { get; set; }
        public bool ValidatorFound { get; set; }

        public override string ToString()
        {
            var inf = SkipType == ValidationSkipType.No
                ? ValidatorFound
                    ? "found"
                    : "miss"
                : "skip";
            return $"{ParameterInfo.Name}: {inf}";
        }
    }

    public enum ValidationSkipType
    {
        No,
        Attribute,
        MemberType,
        Assembly,
    }
}
