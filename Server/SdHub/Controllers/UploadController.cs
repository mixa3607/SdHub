using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SdHub.Database;
using SdHub.Database.Entities.Images;
using SdHub.Database.Entities.User;
using SdHub.Extensions;
using SdHub.Models;
using SdHub.Models.Upload;
using SdHub.Services.FileProc;
using SdHub.Services.User;
using SimpleBase;

namespace SdHub.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[AllowAnonymous]
public class UploadController : ControllerBase
{
    private readonly SdHubDbContext _db;
    private readonly IUserFromTokenService _userFromToken;
    private readonly ILogger<UploadController> _logger;
    private readonly IFileProcessor _fileProcessor;
    private readonly IMapper _mapper;

    public UploadController(SdHubDbContext db, IUserFromTokenService userFromToken, ILogger<UploadController> logger,
        IFileProcessor fileProcessor, IMapper mapper)
    {
        _db = db;
        _userFromToken = userFromToken;
        _logger = logger;
        _fileProcessor = fileProcessor;
        _mapper = mapper;
    }

    [HttpPost]
    [DisableRequestSizeLimit]
    public async Task<UploadResponse> Upload([FromForm] UploadRequest req, CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();

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

        UserEntity user;

        var jwtUser = _userFromToken.Get();
        if (jwtUser != null)
        {
            var regUser = await _db.Users
                .Include(x => x.Plan)
                .FirstOrDefaultAsync(x => x.DeletedAt == null && x.Guid == jwtUser.Guid && !x.IsAnonymous, ct);
            if (regUser == null)
                ModelState.AddError($"User {jwtUser.Login} not found").ThrowIfNotValid();
            user = regUser!;
        }
        else
        {
            var anonUser = await _db.Users
                .Include(x => x.Plan)
                .FirstOrDefaultAsync(x => x.DeletedAt == null && x.IsAnonymous, ct);
            user = anonUser!;
        }

        var uploadedLastHour = 0;
        if (user.IsAnonymous)
        {
            uploadedLastHour = await _db.Images
                .Where(x => x.DeletedAt == null
                            && x.OwnerId == user.Id
                            && x.CreatedAt > DateTimeOffset.Now.AddHours(-1))
                .CountAsync(ct);
        }
        else
        {
            uploadedLastHour = await _db.Images
                .Where(x => x.DeletedAt == null
                            && x.UploaderId == uploader.Id
                            && x.CreatedAt > DateTimeOffset.Now.AddHours(-1))
                .CountAsync(ct);
        }

        var manageToken = $"mg_{Guid.NewGuid():N}";
        var handledFiles = new List<UploadedFileModel>();
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
                uplFile.ManageToken = manageToken;
                uplFile.Uploaded = true;
                uplFile.Reason = "OK";
                uplFile.Image = _mapper.Map<ImageModel>(imageEntity);
                _db.Images.Add(imageEntity);
            }
            catch (MetadataExtractor.ImageProcessingException e)
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
        return new UploadResponse()
        {
            Files = handledFiles
        };
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