using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HeyRed.Mime;
using SdHub.Database.Entities.Files;
using SdHub.Storage;

namespace SdHub.Services.FileProc;

public interface IFileProcessor
{
    string GetNewTempFilePath();
    string GetNewTempDirPath();

    string MapToHashPath(string? basePath, string hash, string originalName);

    Task<string> WriteToCacheAsync(Stream bytes, CancellationToken ct = default);

    Task<string> CalculateHashAsync(string tempFile, CancellationToken ct = default);
    Task<string> CalculateHashAsync(Stream bytes, CancellationToken ct = default);

    Task<FileType> DetectMimeTypeAsync(Stream dataStream, CancellationToken ct = default);
    Task<FileType> DetectMimeTypeAsync(string path, CancellationToken ct = default);

    Task<FileEntity> UploadAsync(Stream dataStream, string srcFileName, string destinationPath, CancellationToken ct = default);
    Task<FileEntity> UploadAsync(Stream dataStream, string srcFileName, string destinationPath, IFileStorage storage, CancellationToken ct = default);

    Task<ExtractImageMetadataResult> ExtractImageMetadataAsync(string tempFile, CancellationToken ct = default);

    Task<IFileStorage> GetStorageAsync(long requiredBytes = 0, CancellationToken ct = default);
    Task<DecomposeUrlResult> DecomposeUrlAsync(string composedUrl, CancellationToken ct = default);

    Task PruneCacheAsync(DateTime deleteBefore, CancellationToken ct = default);
}