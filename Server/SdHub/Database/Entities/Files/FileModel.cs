using System;

namespace SdHub.Database.Entities.Files;

public class FileModel
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? DirectUrl { get; set; }
    public string? Hash { get; set; }
    public string? MimeType { get; set; }
    public string? Extension { get; set; }
    public int Size { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
}