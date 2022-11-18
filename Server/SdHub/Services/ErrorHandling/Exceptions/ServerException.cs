using System;
using System.Runtime.Serialization;

namespace SdHub.Services.ErrorHandling.Exceptions;

public class ServerException : Exception
{
    public string Title { get; set; } = "";
    public string MoreData { get; set; } = "";

    public ServerException()
        : base() { }

    public ServerException(string message)
        : base(message) { }

    public ServerException(string title, string message)
        : base(message)
    {
        Title = title;
    }

    public ServerException(string format, params object[] args)
        : base(string.Format(format, args)) { }

    public ServerException(string message, Exception innerException)
        : base(message, innerException) { }

    public ServerException(string format, Exception innerException, params object[] args)
        : base(string.Format(format, args), innerException) { }

    protected ServerException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}