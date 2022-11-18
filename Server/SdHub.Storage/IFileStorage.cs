namespace SdHub.Storage;

public interface IFileStorage
{
    public string? Name { get; }
    public string? BaseUrl { get; }
    public FileStorageBackendType BackendType { get; }
    public int Version { get; }

    Task InitAsync(CancellationToken ct = default);

    Task<FileSaveResult> SaveAsync(Stream dataStream, string originalName, CancellationToken ct = default);

    Task<bool> IsAvailableAsync(long requiredBytes = 0, CancellationToken ct = default);
}