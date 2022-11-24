using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SdHub.Database.Entities.Files;
using SdHub.Storage;

namespace SdHub.Services.FileProc;

public interface IFileProcessor
{
    string GetNewTempFilePath();
    Task<string> WriteToCacheAsync(Stream bytes, CancellationToken ct = default);
    Task<ExtractImageMetadataResult> ExtractImageMetadataAsync(string tempFile, CancellationToken ct = default);
    Task DeleteTempFileAsync(string tempFile, CancellationToken ct = default);

    Task<FileSaveResult> WriteFileToStorageAsync(string tempFile, string originalName, 
        CancellationToken ct = default);

    Task<FileSaveResult> WriteFileToStorageAsync(Stream seekableStream, string originalName,
        CancellationToken ct = default);

    Task<FileEntity> SaveToDatabaseAsync(FileSaveResult saveResult, CancellationToken ct = default);
}