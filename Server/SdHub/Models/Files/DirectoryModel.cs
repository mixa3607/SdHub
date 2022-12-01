using System;

namespace SdHub.Models.Files;

public class DirectoryModel
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public long Size { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    public string? DirectUrl { get; set; }
}