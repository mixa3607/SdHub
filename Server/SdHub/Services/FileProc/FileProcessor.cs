using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ImageMagick;
using MetadataExtractor.Formats.Png;
using MetadataExtractor.Formats.WebP;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SdHub.Database;
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
    private readonly SdHubDbContext _db;
    private readonly IMapper _mapper;

    public FileProcessor(ILogger<FileProcessor> logger, IFileStorageFactory fileStorageFactory,
        IImageMetadataExtractor imageMetadataExtractor, IOptions<FileProcessorOptions> options,
        IEnumerable<IMetadataSoftwareDetector> softwareDetectors, SdHubDbContext db, IMapper mapper)
    {
        _logger = logger;
        _fileStorageFactory = fileStorageFactory;
        _imageMetadataExtractor = imageMetadataExtractor;
        _db = db;
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

    public async Task<string> WriteToCacheAsync(Stream bytes, CancellationToken ct = default)
    {
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
        using var sha = SHA256.Create();
        var hashBytes = await sha.ComputeHashAsync(bytes, ct);
        var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        return hash;
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

    public Task DeleteTempFilesAsync(DateTime deleteBefore, CancellationToken ct = default)
    {
        if (_options.PreserveCache)
        {
            _logger.LogWarning("Skip deleting temp files!");
            return Task.CompletedTask;
        }

        foreach (var file in Directory.EnumerateFiles(_options.CacheDir!))
        {
            var fileInfo = new FileInfo(file);
            if (fileInfo.LastAccessTimeUtc < deleteBefore) 
                fileInfo.Delete();
        }

        foreach (var dir in Directory.EnumerateDirectories(_options.CacheDir!))
        {
            var dirInfo = new DirectoryInfo(dir);
            if (dirInfo.LastAccessTimeUtc < deleteBefore)
                dirInfo.Delete();
        }

        return Task.CompletedTask;
    }

    public async Task<FileSaveResult> WriteFileToStorageAsync(string tempFile, string originalName, string hash,
        CancellationToken ct = default)
    {
        await using var stream = File.OpenRead(tempFile);
        return await WriteFileToStorageAsync(stream, originalName, hash, ct);
    }

    public async Task<FileSaveResult> WriteFileToStorageAsync(Stream seekableStream, string originalName, string hash,
        CancellationToken ct = default)
    {
        var storage = await _fileStorageFactory.GetStorageAsync(seekableStream.Length, ct);
        var saveResult = await storage.SaveAsync(seekableStream, originalName, hash, ct);
        return saveResult;
    }

    public async Task<FileEntity> SaveToDatabaseAsync(FileSaveResult saveResult, CancellationToken ct = default)
    {
        var entity = _mapper.Map<FileEntity>(saveResult);
        entity.Storage = await _db.FileStorages.FirstAsync(x => x.Name == entity.StorageName, ct);
        _db.Files.Add(entity);
        await _db.SaveChangesAsync(CancellationToken.None);
        return entity;
    }
}