using System;
using System.Diagnostics;

namespace SdHub.Services.ValidatorsCheck;

[DebuggerDisplay("{Controller.Name}: {Arg.Name}")]
public record MissingValidatorError(Type Controller, Type Arg);