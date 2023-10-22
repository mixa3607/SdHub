using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace SdHub.Storage.S3;

public class S3FileStorage : IFileStorage
{
    private readonly ILogger<S3FileStorage> _logger;
    private readonly S3StorageSettings _settings;
    private IMinioClient _client;

    public FileStorageBackendType BackendType => FileStorageBackendType.S3;
    public string Name { get; }
    public string BaseUrl { get; }
    public int Version { get; }

    public S3FileStorage(ILogger<S3FileStorage> logger, IStorageInfo info, string settings)
    {
        _logger = logger;
        _settings = new S3StorageSettings();
        _settings.Load(settings);
        _client = new MinioClient();
        
        Name = info.Name!;
        BaseUrl = info.BaseUrl!;
        Version = info.Version;
    }


    public async Task InitAsync(CancellationToken ct = default)
    {
        _client = _client
            .WithEndpoint(_settings.Endpoint)
            .WithCredentials(_settings.Login, _settings.Password)
            .WithSSL(_settings.WithSsl)
            .Build();
        var exist = await _client.BucketExistsAsync(new BucketExistsArgs().WithBucket(_settings.BucketName), ct);
        if (exist)
        {
            _logger.LogInformation("Bucket {name} exist. Skip creating", _settings.BucketName);
            return;
        }


        _logger.LogInformation("Creating bucket {name}", _settings.BucketName);
        await _client.MakeBucketAsync(new MakeBucketArgs()
                .WithBucket(_settings.BucketName),
            ct);
        await _client.SetPolicyAsync(new SetPolicyArgs()
                .WithBucket(_settings.BucketName)
                .WithPolicy(_settings.PolicyJson),
            ct);
    }

    public async Task<FileUploadResult> UploadAsync(Stream dataStream, string destination, CancellationToken ct = default)
    {
        dataStream.Position = 0;
        var size = dataStream.Length;
        var s3DstFile = destination.Replace('\\', '/');
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_settings.BucketName)
            .WithObject(s3DstFile)
            .WithObjectSize(dataStream.Length)
            .WithStreamData(dataStream);
        await _client.PutObjectAsync(putObjectArgs, ct);

        return new FileUploadResult(s3DstFile, size, Name);
    }

    public async Task<FileUploadResult?> FileExistAsync(string destination, CancellationToken ct = default)
    {
        var s3DstFile = destination.Replace('\\', '/');
        try
        {
            var stat = await _client.StatObjectAsync(new StatObjectArgs().WithBucket(_settings.BucketName).WithObject(s3DstFile), ct);
            return new FileUploadResult(s3DstFile, stat.Size, Name);
        }
        catch (ObjectNotFoundException)
        {
            return null;
        }
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