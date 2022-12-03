using System;

namespace SdHub.RequestFeatures;

public class JwtAuthFailedFeature
{
    public JwtAuthFailedFeature(Exception exception)
    {
        Exception = exception;
    }

    public Exception Exception { get; }
}