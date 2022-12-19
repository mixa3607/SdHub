using System;

namespace SdHub.ApiTokenAuth;

public class ApiTokenAuthException : Exception
{
    public string? Error { get; set; }
    public string? ErrorDescription { get; set; }
    public string? ErrorUri { get; set; }

    public ApiTokenAuthException(string? error = null, string? errorDescription = null, string? errorUri = null,
        Exception? innerException = null)
        : base(error ?? errorDescription, innerException)
    {
        Error = error;
        ErrorDescription = errorDescription;
        ErrorUri = errorUri;
    }
}