using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;

namespace SdHub.Storage.ReadOnly;

public class ReadOnlyFileStorage : IFileStorage
{
    private readonly ILogger<ReadOnlyFileStorage> _logger;

    public FileStorageBackendType BackendType => FileStorageBackendType.ReadOnly;
    public string Name { get; }
    public string BaseUrl { get; }
    public int Version { get; }

    public ReadOnlyFileStorage(ILogger<ReadOnlyFileStorage> logger, IStorageInfo info, string settings)
    {
        _logger = logger;

        Name = info.Name!;
        BaseUrl = info.BaseUrl!;
        Version = info.Version;
    }

    public Task InitAsync(CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }

    public Task<FileUploadResult> UploadAsync(Stream dataStream, string destination,
        CancellationToken ct = default)
    {
        throw new NotSupportedException("Uploading not supported for ReadOnly storage");
    }

    public async Task<FileUploadResult?> FileExistAsync(string destination, CancellationToken ct = default)
    {
        var url = BaseUrl.AppendPathSegment(destination);
        var req = url.ConfigureRequest(x => { x.AllowedHttpStatusRange = "200,404"; });
        var resp = await req.HeadAsync(ct);
        if (resp.StatusCode == 404)
            return null;
        resp.Headers.TryGetFirst("Content-Length", out var sizeStr);
        if (sizeStr == null || !long.TryParse(sizeStr, out var size))
            throw new Exception("Cant detect file size");

        return new FileUploadResult(destination, size, Name);
    }

    public Task<bool> IsAvailableAsync(long requiredBytes, CancellationToken ct = default)
    {
        return Task.FromResult(requiredBytes == 0);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}