using System.Collections.Generic;
using System.Linq;
using SdHub.Constants;
using SdHub.Models.Image;
using SdHub.Services.FileProc.Metadata;

namespace SdHub.Services.FileProc.Detectors;

public class Automatic1111MetadataSoftwareDetector : IMetadataSoftwareDetector
{
    public string SoftwareName => SoftwareGeneratedTypes.AutomaticWebUi;


    public bool TryExtract(List<ImageParsedMetadataTagModel> imageMetaTags, IReadOnlyList<ImageMetadataDirectory> meta)
    {
        {
            var pngDirName = "PNG-tEXt";
            var pairs = meta
                .Where(x => x.Type == pngDirName)
                .SelectMany(x => x.Tags)
                .Where(x => x.Value.AsString != null)
                .Select(x => x.Value.AsString!.Split(':', 2))
                .Where(x => x.Length == 2)
                .Select(x => new ImageParsedMetadataTagModel()
                {
                    Name = x![0],
                    Value = x[1].Trim(),
                    Source = pngDirName,
                    Software = SoftwareName,
                })
                .ToArray();
            if (pairs.Any(x => x.Name == "parameters"))
            {
                imageMetaTags.AddRange(pairs);
                return true;
            }
        }

        //bad bcs can be overwritten with extra
        {
            var exifDirName = "Exif SubIFD";
            var pairs = meta
                .Where(x => x.Type == exifDirName)
                .SelectMany(x => x.Tags)
                .Where(x => x.Name == "User Comment" && x.Value.AsString?.Contains("Model hash: ") == true)
                .Select(x => new ImageParsedMetadataTagModel()
                {
                    Name = "Old params",
                    Value = x.Value!.AsString!.Trim(),
                    Source = exifDirName,
                    Software = SoftwareName,
                })
                .ToArray();
            if (pairs.Length > 0)
            {
                imageMetaTags.AddRange(pairs);
                return true;
            }
        }

        //https://github.com/AUTOMATIC1111/stable-diffusion-webui/pull/2747
        {
            var exifDirName = "Exif SubIFD";
            const string terminator = "\n####################";
            const string delimiter = "\n####################\n";
            var pairs = meta
                .Where(x => x.Type == exifDirName)
                .SelectMany(x => x.Tags)
                .Where(x => x.Name == "User Comment" && x.Value.AsString?.EndsWith(terminator) == true)
                .SelectMany(x => x.Value.AsString![..^terminator.Length].Split(delimiter))
                .Select(x => x.Split(':', 2))
                .Where(x => x.Length == 2)
                .Select(x => new ImageParsedMetadataTagModel()
                {
                    Name = x![0],
                    Value = x[1].Trim(),
                    Source = exifDirName,
                    Software = SoftwareName,
                })
                .ToArray();
            if (pairs.Any(x => x.Name == "parameters"))
            {
                imageMetaTags.AddRange(pairs);
                return true;
            }
        }

        return false;
    }
}