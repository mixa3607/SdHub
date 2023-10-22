using System.Net;

namespace SdHub.Shared.AspErrorHandling;

public class ServerErrorResponse
{
    public HttpStatusCode StatusCode { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }
    public string? MoreData { get; set; }
    public string? InnerException { get; set; }
    public string? TraceIdentifier { get; set; }

    public IReadOnlyDictionary<string, IReadOnlyList<string>> ModelState { get; set; } =
        new Dictionary<string, IReadOnlyList<string>>();
}
