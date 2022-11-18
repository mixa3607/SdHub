using SdHub.Storage;

namespace SdHub.Database.Entities.Files;

public class FileStorageEntity : IStorageInfo
{
    public string? Name { get; set; }
    public string? BaseUrl { get; set; }
    public int Version { get; set; }

    public FileStorageBackendType BackendType { get; set; }
    public string? Settings { get; set; }
}