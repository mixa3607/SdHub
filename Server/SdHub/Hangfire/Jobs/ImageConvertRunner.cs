using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Flurl.Http;
using ImageMagick;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SdHub.Database;
using SdHub.Models;
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

    [DisplayName("Gen images for {0}")]
    public async Task GenerateImagesAsync(long imageId, CancellationToken ct = default)
    {
        var image = await _db.Images
            .Include(x => x.OriginalImage)
            .FirstOrDefaultAsync(x => x.Id == imageId, ct);
        if (image == null)
            return;

        var imageModel = _mapper.Map<ImageModel>(image);
        if (imageModel.ThumbImage == null)
        {
            var srcImgStream = await imageModel.OriginalImage!.DirectUrl.GetStreamAsync(ct);
            using var srcImage = new MagickImage(srcImgStream);
            if (srcImage.Width > 512 || srcImage.Height > 512) 
                srcImage.Resize(new MagickGeometry("512x512>"));

            using var dstImgStream = new MemoryStream();
            await srcImage.WriteAsync(dstImgStream, MagickFormat.WebP, ct);
            dstImgStream.Position = 0;

            var result = await _fileProcessor.WriteFileToStorageAsync(dstImgStream, $"{imageId}_thumb.webp", ct);
            var file = await _fileProcessor.SaveToDatabaseAsync(result, CancellationToken.None);
            image.ThumbImage = file;
            await _db.SaveChangesAsync(CancellationToken.None);
        }
    }
}