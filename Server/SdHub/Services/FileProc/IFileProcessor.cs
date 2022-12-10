using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SdHub.Database.Entities.Files;
using SdHub.Storage;

namespace SdHub.Services.FileProc;

public interface IFileProcessor
{
    string GetNewTempFilePath();
    string GetNewTempDirPath();

    Task<string> WriteToCacheAsync(Stream bytes, CancellationToken ct = default);

    Task<string> CalculateHashAsync(string tempFile, CancellationToken ct = default);
    Task<string> CalculateHashAsync(Stream bytes, CancellationToken ct = default);

    Task<ExtractImageMetadataResult> ExtractImageMetadataAsync(string tempFile, CancellationToken ct = default);


    Task<FileSaveResult> WriteFileToStorageAsync(string tempFile, string originalName, string hash,
        CancellationToken ct = default);

    Task<FileSaveResult> WriteFileToStorageAsync(Stream seekableStream, string originalName, string hash,
        CancellationToken ct = default);

    Task DeleteTempFilesAsync(DateTime deleteBefore, CancellationToken ct = default);

    Task<FileEntity> SaveToDatabaseAsync(FileSaveResult saveResult, CancellationToken ct = default);
}