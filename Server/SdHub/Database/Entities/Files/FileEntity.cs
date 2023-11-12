using System;

namespace SdHub.Database.Entities.Files;

public class FileEntity
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public string? MimeType { get; set; }
    public string? Extension { get; set; }
    public string? Hash { get; set; }
    public long Size { get; set; }

    public string? StorageName { get; set; }
    public FileStorageEntity? Storage { get; set; }
    public string? PathOnStorage { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}