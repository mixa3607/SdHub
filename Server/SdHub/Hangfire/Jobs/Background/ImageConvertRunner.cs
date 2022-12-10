using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Flurl.Http;
using ImageMagick;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SdHub.Database;
using SdHub.Models.Image;
using SdHub.Services.FileProc;

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

            dstImgStream.Position = 0;
            var hash = await _fileProcessor.CalculateHashAsync(dstImgStream, ct);

            dstImgStream.Position = 0;
            var result = await _fileProcessor.WriteFileToStorageAsync(dstImgStream, $"{imageId}_thumb.webp", hash, ct);
            var file = await _fileProcessor.SaveToDatabaseAsync(result, CancellationToken.None);
            image.ThumbImage = file;
            await _db.SaveChangesAsync(CancellationToken.None);
        }

        if (image.CompressedImageId == null || force)
        {
            using var srcImage = new MagickImage(srcImgStream);
            using var dstImgStream = new MemoryStream();
            await srcImage.WriteAsync(dstImgStream, MagickFormat.WebP, ct);

            dstImgStream.Position = 0;
            var hash = await _fileProcessor.CalculateHashAsync(dstImgStream, ct);

            dstImgStream.Position = 0;
            var result = await _fileProcessor.WriteFileToStorageAsync(dstImgStream, $"{imageId}_compressed.webp", hash, ct);
            var file = await _fileProcessor.SaveToDatabaseAsync(result, CancellationToken.None);
            image.CompressedImage = file;
            await _db.SaveChangesAsync(CancellationToken.None);
        }
    }

    public Task GenerateGridAsync(long imageId, bool force, CancellationToken ct = default)
    {
        throw new System.NotImplementedException();
    }
}