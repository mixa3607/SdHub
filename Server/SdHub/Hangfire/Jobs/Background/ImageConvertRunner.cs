using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Flurl.Http;
using ImageMagick;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SdHub.Database;
using SdHub.Database.Entities.Files;
using SdHub.Database.Extensions;
using SdHub.Models.Image;
using SdHub.Services.FileProc;
using SdHub.Services.GridProc;

namespace SdHub.Hangfire.Jobs;

public class ImageConvertRunner : IImageConvertRunnerV1
{
    public string Name => "ImageConverter";

    private readonly SdHubDbContext _db;
    private readonly IFileProcessor _fileProcessor;
    private readonly ILogger<ImageConvertRunner> _logger;
    private readonly IMapper _mapper;

    public ImageConvertRunner(SdHubDbContext db, IFileProcessor fileProcessor, ILogger<ImageConvertRunner> logger,
        IMapper mapper)
    {
        _db = db;
        _fileProcessor = fileProcessor;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task GenerateImagesAsync(long imageId, bool force, CancellationToken ct = default)
    {
        var image = await _db.Images
            .Include(x => x.OriginalImage)
            .FirstOrDefaultAsync(x => x.Id == imageId, ct);
        if (image == null)
            return;

        var imageModel = _mapper.Map<ImageModel>(image);
        using var srcImgStream = new MemoryStream();
        if (image.ThumbImageId == null || image.CompressedImageId == null || force)
        {
            var dwnStream = await imageModel.OriginalImage!.DirectUrl.GetStreamAsync(ct);
            await dwnStream.CopyToAsync(srcImgStream, ct);
            srcImgStream.Position = 0;
        }

        if (image.ThumbImageId == null || force)
        {
            using var srcImage = new MagickImage(srcImgStream);
            if (srcImage.Width > 512 || srcImage.Height > 512)
                srcImage.Resize(new MagickGeometry("512x512>"));

            using var dstImgStream = new MemoryStream();
            await srcImage.WriteAsync(dstImgStream, MagickFormat.WebP, ct);

            var srcName = $"img_{imageId}_thumb.webp";
            var hash = await _fileProcessor.CalculateHashAsync(dstImgStream, ct);
            var dstPath = _fileProcessor.MapToHashPath("thumbs", hash, srcName);
            var file = await _fileProcessor.UploadAsync(dstImgStream, srcName, dstPath, ct);

            image.ThumbImage = file;
            await _db.SaveChangesAsync(CancellationToken.None);
        }

        if (image.CompressedImageId == null || force)
        {
            using var srcImage = new MagickImage(srcImgStream);
            using var dstImgStream = new MemoryStream();
            await srcImage.WriteAsync(dstImgStream, MagickFormat.WebP, ct);

            var srcName = $"img_{imageId}_compressed.webp";
            var hash = await _fileProcessor.CalculateHashAsync(dstImgStream, ct);
            var dstPath = _fileProcessor.MapToHashPath("cmpr", hash, srcName);
            var file = await _fileProcessor.UploadAsync(dstImgStream, srcName, dstPath, ct);

            image.CompressedImage = file;
            await _db.SaveChangesAsync(CancellationToken.None);
        }
    }

    public async Task GenerateGridAsync(long gridId, bool force, CancellationToken ct = default)
    {
        var grid = await _db.Grids
            .Include(x => x.GridImages)
            .ApplyFilter()
            .FirstOrDefaultAsync(x => x.Id == gridId, ct);
        if (grid == null)
            return;

        var images = await _db.Images
            .Include(x => x.OriginalImage)
            .Where(x => x.GridImage!.GridId == gridId)
            .ToArrayAsync(ct);
        var imageModels = _mapper.Map<ImageModel[]>(images).OrderBy(x => x.OriginalImage!.Name).ToArray();
        var tmpDir = _fileProcessor.GetNewTempDirPath();
        var srcFilesPath = Path.Combine(tmpDir, "src").Replace('\\', '/');
        var layersPath = Path.Combine(tmpDir, "layers").Replace('\\', '/');

        await Parallel.ForEachAsync(imageModels.Select((x, i) => (model: x, idx: i)), new ParallelOptions()
            {
                MaxDegreeOfParallelism = 10,
                CancellationToken = ct
            },
            async (file, ct) =>
            {
                var (image, i) = file;
                var tmpFile = $"{i.ToString().PadLeft(8, '0')}.bin";
                await image.OriginalImage!.DirectUrl.DownloadFileAsync(srcFilesPath, tmpFile, cancellationToken: ct);
            });

        var opts = new TilerOptions()
        {
            XCount = grid.XTiles,
            YCount = grid.YTiles,
            MinLayer = 18,
            LayersRoot = layersPath,
            SourceDir = srcFilesPath,
        };
        var tiler = new Tiler(opts);
        var layers = tiler.BuildLayers();
        foreach (var layer in layers)
        {
            await tiler.ConvertLayerAsync(layer);
        }

        var files = layers.SelectMany(x => x.ConvertsList.Select(y => y.Destination!)).ToArray();
        var storage = await _fileProcessor.GetStorageAsync(ct: ct);
        var dstDir = Path.Combine("grids", gridId.ToString().PadLeft(10, '0'));
        var totalSize = 0L;
        await Parallel.ForEachAsync(files, new ParallelOptions()
            {
                MaxDegreeOfParallelism = 20,
                CancellationToken = ct
            },
            async (file, ct) =>
            {
                await using var fileStream = File.OpenRead(file);
                var dst = Path.Combine(dstDir, Path.GetRelativePath(tmpDir, file));
                var rslt = await storage.UploadAsync(fileStream, dst, ct);
                Interlocked.Add(ref totalSize, rslt.Size);
            });

        {
            var thumbFilePath = layers.MinBy(x => x.ZId)!.ConvertsList.First().Destination!;
            await using var dstImgStream = File.OpenRead(thumbFilePath);
            var srcName = $"grid_{gridId}_compressed.webp";
            var hash = await _fileProcessor.CalculateHashAsync(dstImgStream, ct);
            var dstPath = _fileProcessor.MapToHashPath("cmpr", hash, srcName);
            var file = await _fileProcessor.UploadAsync(dstImgStream, srcName, dstPath, ct);

            grid.ThumbImage = file;
        }

        grid.MinLayer = layers.MinBy(x => x.ZId)!.ZId;
        grid.MaxLayer = layers.MaxBy(x => x.ZId)!.ZId;
        grid.LayersDirectory = new DirectoryEntity()
        {
            Name = $"grid_{gridId}_layers",
            PathOnStorage = dstDir,
            Size = totalSize,
            StorageName = storage.Name,
        };

        await _db.SaveChangesAsync(CancellationToken.None);
    }
}