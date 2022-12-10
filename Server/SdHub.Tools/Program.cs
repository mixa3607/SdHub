using ImageMagick;
using Microsoft.Extensions.Logging.Abstractions;
using SdHub.Database.Entities.Files;
using SdHub.Storage;
using SdHub.Storage.S3;

var docs = OptionsDocsBuilder.Generate();
var md = OptionsDocsBuilder.ToMarkdown(docs);

Console.WriteLine("Hello, World!");

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
