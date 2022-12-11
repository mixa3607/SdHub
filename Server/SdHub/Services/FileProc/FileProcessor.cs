using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HeyRed.Mime;
using ImageMagick;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SdHub.Database.Entities.Files;
using SdHub.Models.Image;
using SdHub.Options;
using SdHub.Services.FileProc.Detectors;
using SdHub.Services.FileProc.Extractor;
using SdHub.Services.Storage;
using SdHub.Storage;

namespace SdHub.Services.FileProc;

public class FileProcessor : IFileProcessor
{
    private readonly ILogger<FileProcessor> _logger;
    private readonly FileProcessorOptions _options;
    private readonly IFileStorageFactory _fileStorageFactory;
    private readonly IReadOnlyList<IMetadataSoftwareDetector> _softwareDetectors;
    private readonly IImageMetadataExtractor _imageMetadataExtractor;
    private readonly IMapper _mapper;

    public FileProcessor(ILogger<FileProcessor> logger, IFileStorageFactory fileStorageFactory,
        IImageMetadataExtractor imageMetadataExtractor, IOptions<FileProcessorOptions> options,
        IEnumerable<IMetadataSoftwareDetector> softwareDetectors, IMapper mapper)
    {
        _logger = logger;
        _fileStorageFactory = fileStorageFactory;
        _imageMetadataExtractor = imageMetadataExtractor;
        _mapper = mapper;
        _softwareDetectors = softwareDetectors.ToArray();
        _options = options.Value;
        if (!Directory.Exists(_options.CacheDir!))
            Directory.CreateDirectory(_options.CacheDir!);
    }

    public string GetNewTempFilePath()
    {
        var tmpFile = Path.Combine(_options.CacheDir!, Guid.NewGuid().ToString("N"));
        return tmpFile;
    }

    public string GetNewTempDirPath()
    {
        var tmpFile = Path.Combine(_options.CacheDir!, Guid.NewGuid().ToString("N"));
        return tmpFile;
    }

    public string MapToHashPath(string? basePath, string hash, string originalName)
    {
        var extension = Path.GetExtension(originalName);
        var path = Path.Combine(hash[..2], $"{hash}{extension}");
        if (!string.IsNullOrWhiteSpace(basePath))
            path = Path.Combine(basePath, path);
        return path.Replace('\\', '/');
    }

    public async Task<string> WriteToCacheAsync(Stream bytes, CancellationToken ct = default)
    {
        if (bytes.CanSeek)
        {
            bytes.Position = 0;
        }
        var tmpFile = GetNewTempFilePath();
        await using var tmpFileStream = File.OpenWrite(tmpFile);
        await bytes.CopyToAsync(tmpFileStream, ct);
        return tmpFile;
    }

    public async Task<string> CalculateHashAsync(string tempFile, CancellationToken ct = default)
    {
        await using var tmpFileStream = File.OpenRead(tempFile);
        return await CalculateHashAsync(tmpFileStream, ct);
    }

    public async Task<string> CalculateHashAsync(Stream bytes, CancellationToken ct = default)
    {
        bytes.Position = 0;
        using var sha = SHA256.Create();
        var hashBytes = await sha.ComputeHashAsync(bytes, ct);
        var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        return hash;
    }

    public Task<FileType> DetectMimeTypeAsync(Stream dataStream, CancellationToken ct = default)
    {
        dataStream.Position = 0;
        var fileType = MimeGuesser.GuessFileType(dataStream);
        return Task.FromResult(fileType);
    }

    public Task<FileType> DetectMimeTypeAsync(string path, CancellationToken ct = default)
    {
        var fileType = MimeGuesser.GuessFileType(path);
        return Task.FromResult(fileType);
    }

    public async Task<FileEntity> UploadAsync(Stream dataStream, string srcFileName, string destinationPath,
        CancellationToken ct = default)
    {
        var storage = await GetStorageAsync(ct: ct);
        return await UploadAsync(dataStream, srcFileName, destinationPath, storage, ct);
    }

    public async Task<FileEntity> UploadAsync(Stream dataStream, string srcFileName, string destinationPath,
        IFileStorage storage,
        CancellationToken ct = default)
    {
        var mime = await DetectMimeTypeAsync(dataStream, ct);
        var hash = await CalculateHashAsync(dataStream, ct);
        var uplResult = await storage.FileExistAsync(destinationPath, ct);
        uplResult ??= await storage.UploadAsync(dataStream, destinationPath, ct);
        var file = new FileEntity()
        {
            Name = srcFileName,
            StorageName = uplResult.StorageName,
            PathOnStorage = uplResult.PathOnStorage,
            Size = uplResult.Size,
            Extension = mime.Extension,
            MimeType = mime.MimeType,
            Hash = hash,
        };

        return file;
    }

    public async Task<ExtractImageMetadataResult> ExtractImageMetadataAsync(string tempFile,
        CancellationToken ct = default)
    {
        await using var stream = File.OpenRead(tempFile);
        var dirs = _imageMetadataExtractor.ExtractMetadata(stream);

        var height = -1;
        var width = -1;
        try
        {
            stream.Position = 0;
            using var srcImage = new MagickImage(stream);
            height = srcImage.Height;
            width = srcImage.Width;
        }
        catch (Exception)
        {
            // ignore
        }

        if (height == -1 || width == -1)
            _logger.LogWarning("Cant detect width or height in file {file}", tempFile);

        var imageMetaTags = new List<ImageParsedMetadataTagModel>();
        foreach (var softwareDetector in _softwareDetectors)
        {
            if (softwareDetector.TryExtract(imageMetaTags, dirs))
            {
                break;
            }
        }

        return new ExtractImageMetadataResult(imageMetaTags, dirs, height, width);
    }

    public Task PruneCacheAsync(DateTime deleteBefore, CancellationToken ct = default)
    {
        if (_options.PreserveCache)
        {
            _logger.LogWarning("Skip deleting temp files!");
            return Task.CompletedTask;
        }

        foreach (var file in Directory.EnumerateFiles(_options.CacheDir!))
        {
            var fileInfo = new FileInfo(file);
            if (fileInfo.LastWriteTimeUtc < deleteBefore)
                fileInfo.Delete();
        }

        foreach (var dir in Directory.EnumerateDirectories(_options.CacheDir!))
        {
            var dirInfo = new DirectoryInfo(dir);
            if (dirInfo.LastWriteTimeUtc < deleteBefore)
                dirInfo.Delete(true);
        }

        return Task.CompletedTask;
    }

    public Task<IFileStorage> GetStorageAsync(long requiredBytes = 0, CancellationToken ct = default)
    {
        return _fileStorageFactory.GetStorageAsync(requiredBytes, ct);
    }
}