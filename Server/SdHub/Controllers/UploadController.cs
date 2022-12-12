using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Hangfire;
using Hangfire.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SdHub.Constants;
using SdHub.Database;
using SdHub.Database.Entities.Albums;
using SdHub.Database.Entities.Images;
using SdHub.Database.Entities.Users;
using SdHub.Database.Extensions;
using SdHub.Extensions;
using SdHub.Hangfire.Jobs;
using SdHub.Models.Image;
using SdHub.Models.Upload;
using SdHub.Options;
using SdHub.Services.FileProc;
using SdHub.Services.User;
using SimpleBase;

namespace SdHub.Controllers;

[ApiController]
[Route("api/v1/[action]")]
public class UploadController : ControllerBase
{
    private readonly SdHubDbContext _db;
    private readonly AppInfoOptions _appInfo;
    private readonly IUserFromTokenService _userFromToken;
    private readonly ILogger<UploadController> _logger;
    private readonly IFileProcessor _fileProcessor;
    private readonly IMapper _mapper;

    public UploadController(SdHubDbContext db, IUserFromTokenService userFromToken, ILogger<UploadController> logger,
        IFileProcessor fileProcessor, IMapper mapper, IOptions<AppInfoOptions> appInfo)
    {
        _db = db;
        _userFromToken = userFromToken;
        _logger = logger;
        _fileProcessor = fileProcessor;
        _mapper = mapper;
        _appInfo = appInfo.Value;
    }

    [HttpPost]
    [DisableRequestSizeLimit]
    public async Task<UploadResponse> UploadAuth([FromForm] UploadRequest req, CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();
        if (_appInfo.DisableImageUploadAuth) 
            ModelState.AddError("Uploading disabled by administrator").ThrowIfNotValid();
        var uploader = await GetUploaderAsync(ct);

        var jwtUser = _userFromToken.Get();
        var user = await _db.Users
            .Include(x => x.Plan)
            .ApplyFilter(guid: jwtUser!.Guid)
            .FirstAsync(ct);

        var albumId = 0L;
        if (!string.IsNullOrWhiteSpace(req.AlbumShortToken))
        {
            var album = await _db.Albums
                .FirstOrDefaultAsync(x => x.ShortToken == req.AlbumShortToken && x.DeletedAt == null, ct);
            if (album == null)
            {
                ModelState.AddError(ModelStateErrors.AlbumNotFound).ThrowIfNotValid();
            }
            else if (album.OwnerId != user.Id)
            {
                ModelState.AddError(ModelStateErrors.NotAlbumOwner).ThrowIfNotValid();
            }

            albumId = album!.Id;
        }

        var uploadedLastHour = await _db.Images
            .Where(x => x.DeletedAt == null
                        && x.OwnerId == user.Id
                        && x.CreatedAt > DateTimeOffset.Now.AddHours(-1))
            .CountAsync(ct);

        return await UploadAsync(req, uploadedLastHour, user, uploader, albumId, ct);
    }


    [HttpPost]
    [DisableRequestSizeLimit]
    [AllowAnonymous]
    public async Task<UploadResponse> Upload([FromForm] UploadRequest req, CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();
        if (_appInfo.DisableImageUploadAnon)
            ModelState.AddError("Uploading disabled by administrator").ThrowIfNotValid();
        var uploader = await GetUploaderAsync(ct);

        if (!string.IsNullOrWhiteSpace(req.AlbumShortToken))
            ModelState.AddError("Registration required for upload to album").ThrowIfNotValid();

        var user = await _db.Users
            .Include(x => x.Plan)
            .ApplyFilter(anonymous: true)
            .FirstAsync(ct);

        var uploadedLastHour = await _db.Images
            .Where(x => x.DeletedAt == null
                        && x.UploaderId == uploader.Id
                        && x.CreatedAt > DateTimeOffset.Now.AddHours(-1))
            .CountAsync(ct);

        return await UploadAsync(req, uploadedLastHour, user, uploader, 0, ct);
    }

    private async Task<UploadResponse> UploadAsync(UploadRequest req, int uploadedLastHour, UserEntity user,
        ImageUploaderEntity uploader, long albumId, CancellationToken ct = default)
    {
        var ip = HttpContext.Connection.RemoteIpAddress!.ToString();

        var manageToken = user.IsAnonymous ? $"mg_{Guid.NewGuid():N}" : "";
        var handledFiles = new List<UploadedFileModel>();
        var savedImages = new List<ImageEntity>();

        for (var i = 0; i < req.Files.Count; i++)
        {
            var formFile = req.Files[i];
            var uplFile = new UploadedFileModel();
            handledFiles.Add(uplFile);
            try
            {
                if (uploadedLastHour > user.Plan!.ImagesPerHour)
                {
                    uplFile.Uploaded = false;
                    uplFile.Reason =
                        $"Uploaded files per hour ({user.Plan.ImagesPerHour}) quota reached. Register account for extend quota.";
                    continue;
                }

                if (formFile.Length > user.Plan.MaxImageSizeUpload * 1024)
                {
                    uplFile.Uploaded = false;
                    uplFile.Reason =
                        $"Max image size is {user.Plan.MaxImageSizeUpload / 1024}Mb. Register account for extend quota.";
                    continue;
                }

                if (i >= user.Plan.ImagesPerUpload)
                {
                    uplFile.Uploaded = false;
                    uplFile.Reason =
                        $"Max image count per upload is {user.Plan.ImagesPerUpload}. Register account for extend quota.";
                    continue;
                }

                using var formFileStream = formFile.OpenReadStream();
                var tmpFile = await _fileProcessor.WriteToCacheAsync(formFileStream, ct);
                _logger.LogInformation("Try upload {tmpFile} from {ip} as {name}", tmpFile, ip, formFile.FileName);
                var extractMetaResult = await _fileProcessor.ExtractImageMetadataAsync(tmpFile, ct);
                if (extractMetaResult.ParsedTags.Count == 0 && user.Plan.OnlyWithMetadata)
                {
                    uplFile.Uploaded = false;
                    uplFile.Reason = "Can't detect software generated image. Register account for bypass.";
                    await _fileProcessor.DeleteTempFileAsync(tmpFile, ct);
                    continue;
                }

                var uploadResult = await _fileProcessor.WriteFileToStorageAsync(tmpFile, formFile.FileName, ct);
                if (await _db.Images.AnyAsync(x =>
                        x.Owner!.Id == user.Id
                        && x.OriginalImage!.Hash == uploadResult.Hash
                        && x.DeletedAt == null, ct))
                {
                    uplFile.Uploaded = false;
                    uplFile.Reason = "Already uploaded by you.";
                    await _fileProcessor.DeleteTempFileAsync(tmpFile, ct);
                    continue;
                }

                await _fileProcessor.DeleteTempFileAsync(tmpFile, ct);
                var originalFileEntity = await _fileProcessor.SaveToDatabaseAsync(uploadResult, ct);
                var imageEntity = new ImageEntity()
                {
                    Name = "",
                    Description = "",
                    Owner = user,
                    DeletedAt = null,
                    ShortToken = GenerateShortToken(),
                    ManageToken = manageToken,
                    UploaderId = uploader.Id,
                    RawMetadata = new ImageRawMetadataEntity()
                    {
                        Directories = extractMetaResult.RawDirectories
                    },
                    ParsedMetadata = new ImageParsedMetadataEntity()
                    {
                        Tags = _mapper.Map<ImageParsedMetadataTagEntity[]>(extractMetaResult.ParsedTags),
                        Height = extractMetaResult.Height,
                        Width = extractMetaResult.Width,
                    },
                    OriginalImage = originalFileEntity
                };
                if (albumId != 0)
                {
                    _db.AlbumImages.Add(new AlbumImageEntity()
                    {
                        AlbumId = albumId,
                        Image = imageEntity
                    });
                }

                uplFile.ManageToken = manageToken;
                uplFile.Uploaded = true;
                uplFile.Reason = "OK";
                uplFile.Image = _mapper.Map<ImageModel>(imageEntity);
                _db.Images.Add(imageEntity);
                savedImages.Add(imageEntity);
            }
            catch (MetadataExtractor.ImageProcessingException)
            {
                uplFile.Uploaded = false;
                uplFile.Reason = "Allow only images";
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while processing image {name}", formFile.Name);
                uplFile.Uploaded = false;
                uplFile.Reason = "Internal error";
            }
        }

        await _db.SaveChangesAsync(CancellationToken.None);
        foreach (var imgId in savedImages.Select(x => x.Id))
        {
            BackgroundJob.Enqueue<IImageConvertRunnerV1>(x => x.GenerateImagesAsync(imgId, false, default));
        }

        return new UploadResponse()
        {
            Files = handledFiles
        };
    }


    private async Task<ImageUploaderEntity> GetUploaderAsync(CancellationToken ct = default)
    {
        var ip = HttpContext.Connection.RemoteIpAddress!.ToString();
        var uploader = await _db.ImageUploaders.FirstOrDefaultAsync(x => x.IpAddress == ip, ct);
        if (uploader == null)
        {
            uploader = new ImageUploaderEntity()
            {
                IpAddress = ip
            };
            _db.ImageUploaders.Add(uploader);
            await _db.SaveChangesAsync(CancellationToken.None);
        }

        return uploader;
    }

    private string GenerateShortToken()
    {
        var max = long.MaxValue;
        var rng = Random.Shared.NextInt64(max);
        var rngBytes = BitConverter.GetBytes(rng);
        var b58 = Base58.Bitcoin.Encode(rngBytes);
        return b58;
    }
}