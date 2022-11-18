using System.Security.Cryptography;
using HeyRed.Mime;
using Microsoft.Extensions.Logging;

namespace SdHub.Storage.Local;

public class LocalFileStorage : IFileStorage
{
    private readonly ILogger<LocalFileStorage> _logger;
    private readonly LocalStorageSettings _settings;

    public FileStorageBackendType BackendType => FileStorageBackendType.LocalDir;
    public string? Name { get; }
    public string? BaseUrl { get; }
    public int Version { get; }

    public LocalFileStorage(ILogger<LocalFileStorage> logger, IStorageInfo info, string settings)
    {
        _logger = logger;
        _settings = new LocalStorageSettings();
        _settings.Load(settings);

        Name = info.Name;
        BaseUrl = info.BaseUrl;
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

    public async Task<FileSaveResult> SaveAsync(Stream dataStream, string originalName, CancellationToken ct = default)
    {
        var tmpFile = Path.Combine(_settings.TempPath!, Guid.NewGuid().ToString("N"));
        var size = dataStream.Length;

        await using var tmpFileStream = File.Open(tmpFile, FileMode.CreateNew, FileAccess.ReadWrite);
        await dataStream.CopyToAsync(tmpFileStream, ct);
        tmpFileStream.Position = 0;

        using var sha = SHA256.Create();
        var hashBytes = await sha.ComputeHashAsync(tmpFileStream, ct);
        var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        tmpFileStream.Close();

        var fileType = MimeGuesser.GuessFileType(tmpFile);
        var relPath = Path.Combine(hash[..2], $"{hash}.{fileType.Extension}");
        var virtDstFile = Path.Combine(_settings.VirtualRoot!, relPath);
        var physDstFile = Path.Combine(_settings.PhysicalRoot!, relPath);

        if (!File.Exists(physDstFile))
        {
            _logger.LogInformation("File with name {name} already uploaded. Skip", physDstFile);
            var physDstDir = Path.GetDirectoryName(physDstFile)!;
            if (!Directory.Exists(physDstDir))
                Directory.CreateDirectory(physDstDir);

            File.Move(tmpFile, physDstFile);
        }

        File.Delete(tmpFile);

        return new FileSaveResult(
            hash,
            virtDstFile.Replace('\\', '/'),
            Name!, size,
            fileType.Extension, fileType.MimeType,
            originalName);
    }

    public Task<bool> IsAvailableAsync(long requiredBytes = 0, CancellationToken ct = default)
    {
        return Task.FromResult(!_settings.Disable);
    }
}