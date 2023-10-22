using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
using SdHub.Options;
using SdHub.Services.FileProc;
using SdHub.Services.User;
using SdHub.Shared.AspErrorHandling.ModelState;
using SharpCompress.Readers;
using SimpleBase;

namespace SdHub.Controllers;

[ApiController]
[Route("api/v1/[action]")]
public class UploadGridController : ControllerBase
{
    private readonly SdHubDbContext _db;
    private readonly AppInfoOptions _appInfo;
    private readonly IUserFromTokenService _userFromToken;
    private readonly ILogger<UploadController> _logger;
    private readonly IFileProcessor _fileProcessor;
    private readonly IMapper _mapper;

    public UploadGridController(SdHubDbContext db, IUserFromTokenService userFromToken,
        ILogger<UploadController> logger,
        IFileProcessor fileProcessor, IMapper mapper, IOptions<AppInfoOptions> appInfo)
    {
        _db = db;
        _userFromToken = userFromToken;
        _logger = logger;
        _fileProcessor = fileProcessor;
        _mapper = mapper;
        _appInfo = appInfo.Value;
    }

    //https://github.com/AUTOMATIC1111/stable-diffusion-webui/blob/685f9631b56ff8bd43bce24ff5ce0f9a0e9af490/scripts/xy_grid.py#L287
    private static readonly Regex ReRange =
        new Regex(@"\s*([+-]?\s*\d+)\s*-\s*([+-]?\s*\d+)(?:\s*\(([+-]\d+)\s*\))?\s*");

    private static readonly Regex ReRangeFloat =
        new Regex(@"\s*([+-]?\s*\d+(?:.\d*)?)\s*-\s*([+-]?\s*\d+(?:.\d*)?)(?:\s*\(([+-]\d+(?:.\d*)?)\s*\))?\s*");

    private static readonly Regex ReRangeCount =
        new Regex(@"\s*([+-]?\s*\d+)\s*-\s*([+-]?\s*\d+)(?:\s*\[(\d+)\s*\])?\s*");

    private static readonly Regex ReRangeCountFloat =
        new Regex(@"\s*([+-]?\s*\d+(?:.\d*)?)\s*-\s*([+-]?\s*\d+(?:.\d*)?)(?:\s*\[(\d+(?:.\d*)?)\s*\])?\s*");

    private List<int> CreateRange(int start, int stop, int step)
    {
        var list = new List<int>();
        if (step > 0)
        {
            for (; start < stop; start += step)
            {
                list.Add(start);
            }
        }
        else
        {
            for (; start > stop; start += step)
            {
                list.Add(start);
            }
        }

        return list;
    }

    private List<string> MapPlotToValues(string src)
    {
        var values = new List<string>();
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false
        };
        using var reader = new StringReader(src);
        using var csv = new CsvParser(reader, config);
        while (csv.Read())
        {
            var csvVals = csv.Record;
            if (csvVals == null || csvVals.Length == 0)
                continue;
            foreach (var csvVal in csvVals)
            {
                var m = ReRange.Match(csvVal);
                var mc = ReRangeCount.Match(csvVal);
                //if (m.Success)
                //{
                //    var start = int.Parse(m.Groups[1].Value);
                //    var end = int.Parse(m.Groups[2].Value);
                //    var step = m.Groups[3].Success ? int.Parse(m.Groups[3].Value) : 1;
                //    var vals = Enumerable.Range(start, end).Where(x => x % step == 0).Select(x => x.ToString());
                //    values.AddRange(vals);
                //}
                //else if (mc.Success)
                //{
                //    var start = int.Parse(mc.Groups[1].Value);
                //    var end = int.Parse(mc.Groups[2].Value);
                //    var step = mc.Groups[3].Success ? int.Parse(mc.Groups[3].Value) : 1;
                //    var vals = CreateRange(start, end, step).Select(x => x.ToString());
                //    values.AddRange(vals);
                //}
                //else
                {
                    values.Add(csvVal);
                }
            }
        }

        return values;
    }

    [HttpPost]
    public async Task UploadGridCheckInput([FromBody] UploadGridCheckInputRequest req, CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();
        if (_appInfo.DisableGridUploadAuth)
            ModelState.AddError("Uploading disabled by administrator").ThrowIfNotValid();

        if (req.XTiles * req.YTiles < 2)
            ModelState.AddError("Grid must contain minimum 2 images");

        var xVals = MapPlotToValues(req.XValues!);
        var yVals = MapPlotToValues(req.YValues!);
        if (xVals.Count != req.XTiles)
            ModelState.AddError($"Found {xVals.Count} values on X but expected {req.XTiles}");
        if (yVals.Count != req.YTiles)
            ModelState.AddError($"Found {yVals.Count} values on Y but expected {req.YTiles}");

        ModelState.ThrowIfNotValid();
    }

    [HttpPost]
    [DisableRequestSizeLimit]
    [RequestFormLimits(MultipartBodyLengthLimit = 1024 * 1024 * 1024 * 10L)]
    public async Task<UploadGridResponse> UploadGridAuth([FromForm] UploadGridRequest req,
        CancellationToken ct = default)
    {
        ModelState.ThrowIfNotValid();
        if (_appInfo.DisableGridUploadAuth)
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
            var album = await _db.Albums.ApplyFilter(shortCode: req.AlbumShortToken).FirstOrDefaultAsync(ct);
            if (album == null)
                ModelState.AddError(ModelStateErrors.AlbumNotFound).ThrowIfNotValid();
            else if (album.OwnerId != user.Id)
                ModelState.AddError(ModelStateErrors.NotAlbumOwner).ThrowIfNotValid();

            albumId = album!.Id;
        }

        var uploadedLastHour = await _db.Grids.ApplyFilter(ownerId: user.Id)
            .Where(x => x.CreatedAt > DateTimeOffset.UtcNow.AddHours(-1))
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

    // TODO когда нибудь я разберу эту махину, но не сегодня
    private async Task<UploadGridResponse> UploadAsync(UploadGridRequest req, int uploadedLastHour, UserEntity user,
        ImageUploaderEntity uploader, long albumId, CancellationToken ct = default)
    {
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


        var count = 0;
        await using var formStream = req.File!.OpenReadStream();
        {
            try
            {
                using var archive = ReaderFactory.Open(formStream, new ReaderOptions()
                {
                    LeaveStreamOpen = true
                });
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

                if (count < 1)
                {
                    resp.Uploaded = false;
                    resp.Reason = $"Min images per grid is 2";
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

        if (req.XTiles * req.YTiles != count)
        {
            resp.Uploaded = false;
            resp.Reason = $"Archive must contain {req.XTiles * req.YTiles} images. Read {count}";
            return resp;
        }

        var files = new List<ProcessingFile>();
        formStream.Position = 0;
        {
            using var archive = ReaderFactory.Open(formStream, new ReaderOptions()
            {
                LeaveStreamOpen = true
            });
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

        files = files.OrderBy(x => x.OriginalName).ToList();
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

        await Parallel.ForEachAsync(files,
            new ParallelOptions()
            {
                MaxDegreeOfParallelism = 10,
                CancellationToken = ct
            }, async (file, ct) =>
            {
                if (file.Image != null)
                    return;

                await using var tmpFileStream = System.IO.File.OpenRead(file.TmpFile);
                var hash = await _fileProcessor.CalculateHashAsync(tmpFileStream, ct);
                var dstPath = _fileProcessor.MapToHashPath("img_src", hash, file.OriginalName);
                var uploadResult = await _fileProcessor.UploadAsync(tmpFileStream, file.OriginalName, dstPath, ct);
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
            });

        var grid = new GridEntity()
        {
            ShortToken = GenerateShortToken(),
            OwnerId = user.Id,
            Name = "",
            Description = "",
            XTiles = req.XTiles,
            YTiles = req.YTiles,
            XValues = MapPlotToValues(req.XValues),
            YValues = MapPlotToValues(req.YValues),
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