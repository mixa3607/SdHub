using System;

namespace SdHub.Models.Files;

public class FileModel
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? Hash { get; set; }
    public string? MimeType { get; set; }
    public string? Extension { get; set; }
    public long Size { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? DirectUrl { get; set; }
}