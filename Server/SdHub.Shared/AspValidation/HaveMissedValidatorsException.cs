using System.Text;

namespace SdHub.Shared.AspValidation;

public class HaveMissedValidatorsException : Exception
{
    public IReadOnlyList<ControllerActionValidationResult> Results { get; }

    public HaveMissedValidatorsException(IReadOnlyList<ControllerActionValidationResult> results) : base(
        BuildMessage(results))
    {
        Results = results;
    }

    private static string BuildMessage(IReadOnlyList<ControllerActionValidationResult> results)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Detect missing validators in controllers");
        foreach (var cav in results)
        {
            sb.AppendLine(cav.ToString());
        }

        return sb.ToString();
    }
}
