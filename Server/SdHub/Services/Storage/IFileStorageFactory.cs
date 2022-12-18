using System.Threading;
using System.Threading.Tasks;
using SdHub.Storage;

namespace SdHub.Services.Storage;

public interface IFileStorageFactory
{
    Task<IFileStorage> GetStorageAsync(string composedUrl, CancellationToken ct = default);
    Task<IFileStorage> GetStorageAsync(long requiredBytes = 0, CancellationToken ct = default);
}