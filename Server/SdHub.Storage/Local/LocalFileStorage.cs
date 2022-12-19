using Microsoft.Extensions.Logging;

namespace SdHub.Storage.Local;

public class LocalFileStorage : IFileStorage
{
    private readonly ILogger<LocalFileStorage> _logger;
    private readonly LocalStorageSettings _settings;

    public FileStorageBackendType BackendType => FileStorageBackendType.LocalDir;
    public string Name { get; }
    public string BaseUrl { get; }
    public int Version { get; }

    public LocalFileStorage(ILogger<LocalFileStorage> logger, IStorageInfo info, string settings)
    {
        _logger = logger;
        _settings = new LocalStorageSettings();
        _settings.Load(settings);

        Name = info.Name!;
        BaseUrl = info.BaseUrl!;
        Version = info.Version;
    }


    public Task InitAsync(CancellationToken ct = default)
    {
        if (!Directory.Exists(_settings.TempPath))
            Directory.CreateDirectory(_settings.TempPath!);
        if (!Directory.Exists(_settings.PhysicalRoot))
            Directory.CreateDirectory(_settings.PhysicalRoot!);
        return Task.CompletedTask;
    }

    public async Task<FileUploadResult> UploadAsync(Stream dataStream, string destination,
        CancellationToken ct = default)
    {
        dataStream.Position = 0;
        var virtDstFile = Path.Combine(_settings.VirtualRoot!, destination);
        var physDstFile = Path.Combine(_settings.PhysicalRoot!, destination);
        await using var destFile = File.Create(physDstFile);
        await dataStream.CopyToAsync(destFile, ct);
        return new FileUploadResult(virtDstFile.Replace('\\', '/'), dataStream.Length, Name);
    }

    public Task<FileUploadResult?> FileExistAsync(string destination, CancellationToken ct = default)
    {
        var relPath = Path.GetRelativePath(_settings.VirtualRoot!, destination);
        var physDstFile = Path.Combine(_settings.PhysicalRoot!, relPath);
        var fInfo = new FileInfo(Path.GetFullPath(physDstFile));
        return fInfo.Exists
            ? Task.FromResult<FileUploadResult?>(new FileUploadResult(destination, fInfo.Length, Name))
            : Task.FromResult<FileUploadResult?>(null);
    }

    public Task<bool> IsAvailableAsync(long requiredBytes = 0, CancellationToken ct = default)
    {
        return Task.FromResult(!_settings.Disabled);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}