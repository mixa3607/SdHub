using System;
using System.Collections.Generic;
using System.Text;

namespace SdHub.Services.ValidatorsCheck;

public class HaveMissedValidatorsException : Exception
{
    public IReadOnlyList<MissingValidatorError> MissingValidator { get; }

    public HaveMissedValidatorsException(IReadOnlyList<MissingValidatorError> missingValidator) : base(
        BuildMessage(missingValidator))
    {
        MissingValidator = missingValidator;
    }

    private static string BuildMessage(IReadOnlyList<MissingValidatorError> errs)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Detect {errs.Count} missing validators in controllers");
        foreach (var err in errs)
        {
            sb.AppendLine($"{err.Controller.Name} => {err.Arg.Name}");
        }

        return sb.ToString();
    }
}