namespace SdHub.Storage;

public interface IFileStorage: IAsyncDisposable
{
    public string Name { get; }
    public string BaseUrl { get; }
    public FileStorageBackendType BackendType { get; }
    public int Version { get; }

    Task InitAsync(CancellationToken ct = default);


    Task<FileUploadResult> UploadAsync(Stream dataStream, string destination, CancellationToken ct = default);
    Task<FileUploadResult?> FileExistAsync(string destination, CancellationToken ct = default);

    Task<bool> IsAvailableAsync(long minAvailableKBytes = 0, CancellationToken ct = default);
}