using System.Security.Cryptography;
using HeyRed.Mime;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.Exceptions;

namespace SdHub.Storage.S3;

public class S3FileStorage : IFileStorage
{
    private readonly ILogger<S3FileStorage> _logger;
    private readonly S3StorageSettings _settings;
    private MinioClient _client;

    public FileStorageBackendType BackendType => FileStorageBackendType.S3;
    public string? Name { get; }
    public string? BaseUrl { get; }
    public int Version { get; }

    public S3FileStorage(ILogger<S3FileStorage> logger, IStorageInfo info, string settings)
    {
        _logger = logger;
        _settings = new S3StorageSettings();
        _settings.Load(settings);
        _client = new MinioClient();

        Name = info.Name;
        BaseUrl = info.BaseUrl;
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

    public async Task<FileSaveResult> SaveAsync(Stream dataStream, string originalName, string hash, CancellationToken ct = default)
    {
        var size = dataStream.Length;
        dataStream.Position = 0;

        var fileType = MimeGuesser.GuessFileType(dataStream);
        dataStream.Position = 0;

        var relPath = Path.Combine(hash[..2], $"{hash}.{fileType.Extension}");
        var virtDstFile = Path.Combine("bins", relPath).Replace('\\', '/');
        var s3DstFile = Path.Combine("bins", relPath).Replace('\\', '/');

        bool alreadyUploaded;
        try
        {
            var stat = await _client.StatObjectAsync(new StatObjectArgs().WithBucket(_settings.BucketName).WithObject(s3DstFile), ct);
            alreadyUploaded = true;
        }
        catch (ObjectNotFoundException)
        {
            alreadyUploaded = false;
        }

        if (alreadyUploaded)
        {
            _logger.LogInformation("File with name {name} already uploaded. Skip", s3DstFile);
        }
        else
        {
            var putObjectArgs = new PutObjectArgs()
                .WithBucket(_settings.BucketName)
                .WithObject(s3DstFile)
                .WithObjectSize(dataStream.Length)
                .WithStreamData(dataStream);
            await _client.PutObjectAsync(putObjectArgs, ct);
        }

        return new FileSaveResult(
            hash,
            virtDstFile,
            Name!, size,
            fileType.Extension, fileType.MimeType,
            originalName);
    }

    public Task<bool> IsAvailableAsync(long requiredBytes = 0, CancellationToken ct = default)
    {
        return Task.FromResult(!_settings.Disabled);
    }
}