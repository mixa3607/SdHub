using System.Net;

namespace SdHub.Shared.AspErrorHandling.Exceptions;

public class ServerException : Exception
{
    public string Title { get; set; } = "";
    public string MoreData { get; set; } = "";
    public HttpStatusCode StatusCode { get; } = HttpStatusCode.InternalServerError;

    public ServerException()
        : base()
    {
    }

    public ServerException(string message)
        : base(message)
    {
    }

    public ServerException(string title, string message)
        : base(message)
    {
        Title = title;
    }

    public ServerException(string title, string message, HttpStatusCode statusCode)
        : base(message)
    {
        Title = title;
        StatusCode = statusCode;
    }

    public ServerException(string title, string message, HttpStatusCode statusCode, Exception innerException)
        : base(message, innerException)
    {
        Title = title;
        StatusCode = statusCode;
    }
}
