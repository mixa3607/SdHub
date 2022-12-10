using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SdHub.Constants;
using SdHub.Database;
using SdHub.Database.Entities.Albums;
using SdHub.Database.Entities.Files;
using SdHub.Database.Entities.Grids;
using SdHub.Database.Entities.Images;
using SdHub.Database.Entities.Users;
using SdHub.Database.Extensions;
using SdHub.Extensions;
using SdHub.Hangfire.Jobs;
using SdHub.Models.Grid;
using SdHub.Models.Upload;
using SdHub.Services.FileProc;
using SdHub.Services.User;
using SharpCompress.Readers;
using SimpleBase;

namespace SdHub.Controllers;

[ApiController]
[Route("api/v1/[action]")]
public class UploadGridController : ControllerBase
{
    private readonly SdHubDbContext _db;
    private readonly IUserFromTokenService _userFromToken;
    private readonly ILogger<UploadController> _logger;
    private readonly IFileProcessor _fileProcessor;
    private readonly IMapper _mapper;

    public UploadGridController(SdHubDbContext db, IUserFromTokenService userFromToken,
        ILogger<UploadController> logger,
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
    public async Task<UploadGridResponse> UploadGridAuth([FromForm] UploadGridRequest req,
        CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();
        var uploader = await GetUploaderAsync(ct);

        var jwtUser = _userFromToken.Get();
        var user = await _db.Users
            .Include(x => x.Plan)
            .ApplyFilter(guid: jwtUser!.Guid)
            .FirstAsync(ct);

        var albumId = 0L;
        if (!string.IsNullOrWhiteSpace(req.AlbumShortToken))
        {
            var album = await _db.Albums.ApplyFilter(shortCode: req.AlbumShortToken).FirstOrDefaultAsync(ct);
            if (album == null)
                ModelState.AddError(ModelStateErrors.AlbumNotFound).ThrowIfNotValid();
            else if (album.OwnerId != user.Id)
                ModelState.AddError(ModelStateErrors.NotAlbumOwner).ThrowIfNotValid();

            albumId = album!.Id;
        }

        var uploadedLastHour = await _db.Grids.ApplyFilter(ownerId: user.Id)
            .Where(x => x.CreatedAt > DateTimeOffset.Now.AddHours(-1))
            .CountAsync(ct);

        return await UploadAsync(req, uploadedLastHour, user, uploader, albumId, ct);
    }

    private class ProcessingFile
    {
        public string OriginalName;
        public string TmpFile;
        public ExtractImageMetadataResult Metadata;
        public int Order;
        public string? Hash;
        public ImageEntity? Image;

        public ProcessingFile(string originalName, string tmpFile, ExtractImageMetadataResult metadata, int order)
        {
            OriginalName = originalName;
            TmpFile = tmpFile;
            Metadata = metadata;
            Order = order;
        }
    }

    private async Task<UploadGridResponse> UploadAsync(UploadGridRequest req, int uploadedLastHour, UserEntity user,
        ImageUploaderEntity uploader, long albumId, CancellationToken ct = default)
    {
        var ip = HttpContext.Connection.RemoteIpAddress!.ToString();
        var resp = new UploadGridResponse();

        if (req.File!.Length > user.Plan!.MaxGridArchiveSizeUpload * 1024)
        {
            resp.Uploaded = false;
            resp.Reason = $"Max grid archive size is {user.Plan.MaxGridArchiveSizeUpload / 1024}Mb";
            return resp;
        }

        if (uploadedLastHour > user.Plan!.GridsPerHour)
        {
            resp.Uploaded = false;
            resp.Reason = $"Uploaded grids per hour ({user.Plan.GridsPerHour}) quota reached";
            return resp;
        }

        await using var formStream = req.File!.OpenReadStream();
        {
            try
            {
                using var archive = ReaderFactory.Open(formStream);
                var count = 0;
                while (archive.MoveToNextEntry())
                {
                    if (archive.Entry.IsDirectory)
                        continue;
                    count++;
                }

                if (count > user.Plan!.ImagesPerGrid)
                {
                    resp.Uploaded = false;
                    resp.Reason = $"Max images per grid is {user.Plan!.ImagesPerGrid}";
                    return resp;
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Can't read archive");
                resp.Uploaded = false;
                resp.Reason = $"Can't read archive";
                return resp;
            }
        }

        var files = new List<ProcessingFile>();
        formStream.Position = 0;
        {
            using var archive = ReaderFactory.Open(formStream);
            var i = 0;
            while (archive.MoveToNextEntry())
            {
                if (archive.Entry.IsDirectory)
                    continue;
                try
                {
                    await using var entryStream = archive.OpenEntryStream();
                    var tmpFile = await _fileProcessor.WriteToCacheAsync(entryStream, ct);
                    var extractMetaResult = await _fileProcessor.ExtractImageMetadataAsync(tmpFile, ct);

                    if (extractMetaResult.ParsedTags.Count == 0)
                    {
                        resp.Uploaded = false;
                        resp.Reason = $"Can't detect software for {archive.Entry.Key}";
                        return resp;
                    }

                    files.Add(new ProcessingFile(archive.Entry.Key, tmpFile, extractMetaResult, i));
                    i++;
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, "Can't read file {name}", archive.Entry.Key);
                    resp.Uploaded = false;
                    resp.Reason = $"Can't read file {archive.Entry.Key}";
                    return resp;
                }
            }
        }

        foreach (var f in files)
        {
            f.Hash = await _fileProcessor.CalculateHashAsync(f.TmpFile, ct);
        }

        var hashes = files.Select(x => x.Hash).ToArray();
        var existedImages = await _db.Images
            .Include(x => x.OriginalImage)
            .ApplyFilter(ownerId: user.Id)
            .Where(x => hashes.Contains(x.OriginalImage!.Hash))
            .ToArrayAsync(ct);

        var existedImagesIds = existedImages.Select(x => x.Id).ToArray();
        var existedImagesInGrids = await _db.Images
            .ApplyFilter(ownerId: user.Id)
            .Where(x => existedImagesIds.Contains(x.Id))
            .Where(x => x.GridImage != null)
            .ToArrayAsync(ct);

        if (existedImagesInGrids.Length > 0)
        {
            resp.Uploaded = false;
            resp.Reason = $"Archive contain already uploaded images that assign to another grid";
            return resp;
        }

        foreach (var existedImage in existedImages)
        {
            files.First(x => x.Hash == existedImage.OriginalImage!.Hash).Image = existedImage;
        }

        foreach (var file in files)
        {
            if (file.Image == null)
                continue;
            var uploadResult =
                await _fileProcessor.WriteFileToStorageAsync(file.TmpFile, file.OriginalName, file.Hash!, ct);
            file.Image = new ImageEntity()
            {
                Name = "",
                Description = "",
                Owner = user,
                DeletedAt = null,
                ShortToken = GenerateShortToken(),
                ManageToken = "",
                UploaderId = uploader.Id,
                RawMetadata = new ImageRawMetadataEntity()
                {
                    Directories = file.Metadata.RawDirectories
                },
                ParsedMetadata = new ImageParsedMetadataEntity()
                {
                    Tags = _mapper.Map<ImageParsedMetadataTagEntity[]>(file.Metadata.ParsedTags),
                    Height = file.Metadata.Height,
                    Width = file.Metadata.Width,
                },
                OriginalImage = _mapper.Map<FileEntity>(uploadResult),
            };
        }

        var grid = new GridEntity()
        {
            ShortToken = GenerateShortToken(),
            OwnerId = user.Id,
            Name = "",
            Description = "",
            XTiles = req.XTiles,
            YTiles = req.YTiles,
            XValues = req.XValues,
            YValues = req.YValues,
            GridImages = files.Select(x => new GridImageEntity() { Order = x.Order, Image = x.Image }).ToList(),
        };
        if (albumId != 0)
        {
            grid.AlbumImages = new List<AlbumImageEntity>() { new() { AlbumId = albumId } };
        }

        _db.Grids.Add(grid);
        await _db.SaveChangesAsync(CancellationToken.None);


        foreach (var imgId in grid.GridImages.Select(x => x.ImageId))
        {
            BackgroundJob.Enqueue<IImageConvertRunnerV1>(x => x.GenerateImagesAsync(imgId, false, default));
        }

        BackgroundJob.Enqueue<IImageConvertRunnerV1>(x => x.GenerateGridAsync(grid.Id, false, default));

        grid.AlbumImages = null;
        grid.GridImages = null;

        return new UploadGridResponse()
        {
            Uploaded = true,
            Grid = _mapper.Map<GridModel>(grid)
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