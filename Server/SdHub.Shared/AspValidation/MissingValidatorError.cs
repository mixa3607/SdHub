using System.Diagnostics;

namespace SdHub.Shared.AspValidation;

[DebuggerDisplay("{Controller.Name}: {Arg.Name}")]
public record MissingValidatorError(Type Controller, Type Arg);
