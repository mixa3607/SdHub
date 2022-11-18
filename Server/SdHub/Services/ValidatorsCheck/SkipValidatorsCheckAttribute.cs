using System;

namespace SdHub.Services.ValidatorsCheck;

[AttributeUsage(AttributeTargets.Parameter)]
public class SkipValidatorsCheckAttribute : Attribute
{
    public string Reason { get; }

    public SkipValidatorsCheckAttribute(string reason)
    {
        Reason = reason;
    }
}