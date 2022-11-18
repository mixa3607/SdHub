using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using ImageMagick;
using Minio;
using System.Net.Mime;
using System.Security.AccessControl;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging.Abstractions;
using SdHub.Database.Entities.Files;
using SdHub.Storage;
using SdHub.Storage.S3;


Console.WriteLine("Hello, World!");
var options = new S3StorageSettings()
{
    BucketName = "diffai",
    Login = "diffai",
    Password = "diffaidiffai",
    Disable = false,
    Endpoint = "minio.nas1.in.arkprojects.space",
    WithSsl = true,
    PolicyJson = "{\"Version\":\"2012-10-17\",\"Statement\":[{\"Action\":[\"s3:GetObject\"],\"Effect\":\"Allow\",\"Principal\":{\"AWS\":[\"*\"]},\"Resource\":[\"arn:aws:s3:::diffai/bins/*\"],\"Sid\":\"\"}]}"
};
var ent = new FileStorageEntity()
{
    BackendType = FileStorageBackendType.S3,
    Name = "minio",
    Settings = options.Save(),
    BaseUrl = "",
    Version = 1
};
var storage = new S3FileStorage(new NullLogger<S3FileStorage>(), ent, options.Save());
await storage.InitAsync();
using var fS = File.OpenRead(@"C:\Users\mixa3607\Desktop\Новая папка\tmpxjan9qtd.png");
var uplResult = await storage.SaveAsync(fS, "tmpxjan9qtd.png", CancellationToken.None);
return;

/*
var images = new List<string>()
{
    "./files/grids/raw1/01815.png",
    "./files/grids/raw1/01816.png",
    "./files/grids/raw1/01817.png",
    "./files/grids/raw1/01818.png",
};
var output = "./files/grids/out.png";

using var collection = new MagickImageCollection();
foreach (var image in images)
{
    var iImage = new MagickImage(image);
    iImage.Scale(new Percentage(50));
    collection.Add(iImage);
}

var montageResult = collection.Montage(new MontageSettings()
{
    Geometry = new MagickGeometry("100%x"),
    TileGeometry = new MagickGeometry("2x2")
});
montageResult.Write(output);
*/
var opts = new TilerOptions()
{
    XCount = 27,
    YCount = 81,
    MinLayer = 18,
    //LayersRoot = "./files/grids/layers",
    LayersRoot = @"C:\Users\mixa3607\source\repos\SdHub\Server\SdHub\wwwroot\layers",
    SourceDir = "./files/grids/source",
};
var tiler = new Tiler(opts);
await tiler.DoAsync();
return;


public class TilerOptions
{
    public string? SourceDir { get; set; }
    public string? LayersRoot { get; set; }
    public int XCount { get; set; }
    public int YCount { get; set; }
    public int MinLayer { get; set; }
}

public class Layer
{
    public int ZId { get; set; }
    public string? LayerDir { get; set; }
    public IReadOnlyList<LayerConverts> ConvertsList { get; set; } = Array.Empty<LayerConverts>();
    public IReadOnlyList<LayerRow> Rows { get; set; } = Array.Empty<LayerRow>();
}

public class LayerConverts
{
    public int XTiles { get; set; } = -1;
    public int YTiles { get; set; } = -1;

    public string Geometry = "100%x";
    public IReadOnlyList<string?> Sources { get; set; } = Array.Empty<string>();
    public string? Destination { get; set; }
}

public class LayerRow
{
    public IReadOnlyList<string> Images { get; set; } = Array.Empty<string>();
}

public class Tiler
{
    private readonly TilerOptions _options;

    public Tiler(TilerOptions options)
    {
        _options = options;
    }

    public async Task DoAsync()
    {
        var layer = BuildZeroLayer();
        do
        {
            var layerDir = Path.Combine(_options.LayersRoot!, layer.ZId.ToString());
            if (Directory.Exists(layerDir))
            {
                Console.WriteLine($"Dir for layer {layer.ZId} exist. Skip tiling");
            }
            else
            {
                Directory.CreateDirectory(layerDir);
                Console.WriteLine($"Building tiles for layer {layer.ZId}");
                await ConvertLayerAsync(layer);
            }

            layer = BuildTopLayer(layer);
            Console.WriteLine($"Next layer is {layer?.ZId.ToString() ?? "none"}");
        } while (layer != null);
    }

    public Layer? BuildTopLayer(Layer sourceLayer)
    {
        var tilesInTopX = 2;
        var tilesInTopY = 2;

        var sourceRows = sourceLayer.Rows.Count;
        var sourceCols = sourceLayer.Rows[0].Images.Count;

        if (sourceCols == 1 && sourceRows == 1)
            return null;

        var rowsCount = (sourceRows + tilesInTopY - 1) / tilesInTopY;
        var colsCount = (sourceCols + tilesInTopX - 1) / tilesInTopX;

        var rows = new LayerRow[rowsCount];
        var converts = new List<LayerConverts>(rowsCount * colsCount);
        var layer = new Layer() { Rows = rows, ZId = sourceLayer.ZId - 1, ConvertsList = converts };

        var layerDir = Path.Combine(_options.LayersRoot!, layer.ZId.ToString());

        for (int y = 0; y < rowsCount; y++)
        {
            var rowImages = new string[colsCount];
            rows[y] = new LayerRow() { Images = rowImages };

            for (int x = 0; x < colsCount; x++)
            {
                var destination = Path.Combine(layerDir, $"{x}_{y}.webp");
                rowImages[x] = destination;

                var eX = sourceCols - x * tilesInTopX;
                var dX = eX < tilesInTopX ? eX : tilesInTopX;
                var eY = sourceRows - y * tilesInTopY;
                var dY = eY < tilesInTopY ? eY : tilesInTopY;

                //как же хуёво
                var imgs = sourceLayer.Rows
                    .Skip(y * tilesInTopY).Take(tilesInTopY)
                    .Concat(Enumerable.Repeat(new LayerRow(), tilesInTopY - dY))
                    .SelectMany(r =>
                        r.Images.Skip(x * tilesInTopX).Take(tilesInTopX)
                            .Concat(Enumerable.Repeat<string?>(null, tilesInTopX - dX)))
                    .ToArray();
                //var imgs = sourceLayer.Rows
                //    .Skip(y * tilesInTopY).Take(tilesInTopY)
                //    .SelectMany(r => r.Images.Skip(x * tilesInTopX).Take(tilesInTopX))
                //    .ToArray();

                converts.Add(new LayerConverts()
                {
                    XTiles = tilesInTopX,
                    YTiles = tilesInTopY,
                    Geometry = $"{(100 / tilesInTopX)}%x",
                    Destination = destination,
                    Sources = imgs
                });
            }
        }

        return layer;
    }

    public Layer BuildZeroLayer()
    {
        var files = Directory.GetFiles(_options.SourceDir!).OrderBy(x => x).ToArray();

        var converts = new List<LayerConverts>(files.Length);
        var rows = new LayerRow[_options.YCount];
        var layer = new Layer()
        {
            Rows = rows,
            ConvertsList = converts,
            ZId = _options.MinLayer,
        };
        layer.LayerDir = Path.Combine(_options.LayersRoot!, layer.ZId.ToString());

        for (int y = 0; y < rows.Length; y++)
        {
            var rowImages = new string[_options.XCount];
            rows[y] = new LayerRow() { Images = rowImages };

            for (int x = 0; x < rowImages.Length; x++)
            {
                var source = files[y * rowImages.Length + x];
                var destination = Path.Combine(layer.LayerDir, $"{x}_{y}.webp");
                rowImages[x] = destination;
                converts.Add(new LayerConverts()
                {
                    Destination = destination,
                    Sources = new[] { source }
                });
            }
        }

        return layer;
    }

    public async Task ConvertLayerAsync(Layer layer)
    {
        var handled = 0;
        await Parallel.ForEachAsync(layer.ConvertsList, new ParallelOptions()
        {
            MaxDegreeOfParallelism = 20,
        }, async (conv, ct) =>
        {
            if (conv.Sources.Count == 1)
            {
                var image = conv.Sources[0];
                using var iImage = image != null
                    ? new MagickImage(image)
                    : new MagickImage(new MagickColor(0, 0, 0, 0), 1, 1);
                await iImage.WriteAsync(conv.Destination!, ct);
                Interlocked.Increment(ref handled);
                if (handled % 100 == 0)
                    Console.WriteLine($"Converted {handled} images");
            }
            else
            {
                using var collection = new MagickImageCollection();
                foreach (var image in conv.Sources)
                {
                    MagickImage iImage;
                    if (image == null)
                    {
                        iImage = new MagickImage(new MagickColor(0, 0, 0, 0), 1, 1);
                    }
                    else
                    {
                        iImage = new MagickImage(image);
                        iImage.Scale(new MagickGeometry(conv.Geometry));
                    }

                    collection.Add(iImage);
                }

                var xTiles = conv.XTiles == -1 ? "" : conv.XTiles.ToString();
                var yTiles = conv.YTiles == -1 ? "" : conv.YTiles.ToString();
                var montageResult = collection.Montage(new MontageSettings()
                {
                    Geometry = new MagickGeometry("100%x"),
                    TileGeometry = new MagickGeometry($"{xTiles}x{yTiles}"),
                    BackgroundColor = new MagickColor(0, 0, 0, 0),
                });
                await montageResult.WriteAsync(conv.Destination!, ct);
            }
        });
    }
}