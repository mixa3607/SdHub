using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SdHub.Database;
using SdHub.Database.Entities.Files;
using SdHub.Storage;
using SdHub.Storage.Local;
using SdHub.Storage.S3;

namespace SdHub.Services.Storage;

public class FileStorageFactory : IFileStorageFactory
{
    private readonly ILogger<FileStorageFactory> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly List<IFileStorage> _fileStorages = new List<IFileStorage>();
    private readonly SemaphoreSlim _updateLock = new SemaphoreSlim(1);
    private readonly TimeSpan _updateInterval = TimeSpan.FromMinutes(5);
    private DateTime _lastUpdate;


    public FileStorageFactory(ILogger<FileStorageFactory> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task<IFileStorage> GetStorageAsync(long requiredBytes = 0, CancellationToken ct = default)
    {
        await UpdateStorageListIfRequiredAsync(ct);
        return await GetAvailableStorageAsync(requiredBytes, ct);
    }


    private async Task<IFileStorage> GetAvailableStorageAsync(long requiredBytes = 0, CancellationToken ct = default)
    {
        foreach (var fileStorage in _fileStorages)
        {
            if (await fileStorage.IsAvailableAsync(requiredBytes, ct))
            {
                return fileStorage;
            }
        }

        throw new Exception("No available storage");
    }

    private async Task UpdateStorageListIfRequiredAsync(CancellationToken ct = default)
    {
        await _updateLock.WaitAsync(ct);
        try
        {
            if (DateTime.Now - _lastUpdate < _updateInterval)
                return;
            _logger.LogInformation("Begin update storage list");
            using var sc = _serviceProvider.CreateScope();
            var scp = sc.ServiceProvider;
            var db = scp.GetRequiredService<SdHubDbContext>();

            var entities = await db.FileStorages.ToArrayAsync(ct);
            foreach (var entity in entities)
            {
                var createdStor = _fileStorages.FirstOrDefault(x => x.Name == entity.Name);
                if (createdStor != null && createdStor.Version != entity.Version)
                {
                    _logger.LogInformation("Found new version ({ver}) for storage {name}. Recreate", entity.Version,
                        entity.Name);
                    _fileStorages.Remove(createdStor);
                    var instance = CreateInstance(entity);
                    _fileStorages.Add(instance);
                }
                else if (createdStor != null)
                {
                    _logger.LogInformation("Storage {name} no changes", entity.Name);
                }
                else
                {
                    _logger.LogInformation("Found new storage {name}. Creating", entity.Name);
                    var instance = CreateInstance(entity);
                    await instance.InitAsync(ct);
                    _fileStorages.Add(instance);
                }
            }

            var deletedStorages = _fileStorages.Where(x => entities.All(y => y.Name != x.Name)).ToArray();
            foreach (var deletedStorage in deletedStorages)
            {
                _logger.LogInformation("Delete {name} storage", deletedStorage.Name);
                _fileStorages.Remove(deletedStorage);
            }

            _lastUpdate = DateTime.Now;
            _logger.LogInformation("Success update storage list");
        }
        finally
        {
            _updateLock.Release();
        }
    }

    private IFileStorage CreateInstance(FileStorageEntity entity)
    {
        return entity.BackendType switch
        {
            FileStorageBackendType.LocalDir => new LocalFileStorage(
                _serviceProvider.GetRequiredService<ILogger<LocalFileStorage>>(), entity, entity.Settings!),
            FileStorageBackendType.S3 => new S3FileStorage(
                _serviceProvider.GetRequiredService<ILogger<S3FileStorage>>(), entity, entity.Settings!),
            _ => throw new NotSupportedException($"Backend {entity.BackendType} not supported")
        };
    }
}