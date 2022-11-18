using System;
using System.Collections.Generic;
using System.Net;

namespace SdHub.Services.ErrorHandling;

public class ServerErrorResponse
{
    public HttpStatusCode StatusCode { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }
    public Guid? Guid { get; set; }
    public string? MoreData { get; set; }
    public string? InnerException { get; set; }
    public IReadOnlyDictionary<string, IReadOnlyList<string>> ModelState { get; set; } = new Dictionary<string, IReadOnlyList<string>>();
}