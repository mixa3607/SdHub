using System.Collections.Generic;
using System.Linq;
using MetadataExtractor.Formats.Exif;
using SdHub.Constants;
using SdHub.Models.Image;
using SdHub.Services.FileProc.Metadata;

namespace SdHub.Services.FileProc.Detectors;

public class DreamStudioMetadataSoftwareDetector : IMetadataSoftwareDetector
{
    public string SoftwareName => SoftwareGeneratedTypes.DreamStudio;

    public bool TryExtract(List<ImageParsedMetadataTagModel> imageMetaTags, IReadOnlyList<ImageMetadataDirectory> meta)
    {
        var exifDirName = "Exif IFD0";
        var exifDir = meta.FirstOrDefault(x=>x.Type == exifDirName);
        if (exifDir == null)
            return false;

        var description = exifDir.Tags.FirstOrDefault(x=>x.Type == ExifDirectoryBase.TagImageDescription)?.Value.AsString;
        var model = exifDir.Tags.FirstOrDefault(x=>x.Type == ExifDirectoryBase.TagModel)?.Value.AsString;
        var software = exifDir.Tags.FirstOrDefault(x=>x.Type == ExifDirectoryBase.TagSoftware)?.Value.AsString;

        if (description == null || model == null || software == null)
            return false;

        if (software != "stability.ai")
            return false;

        imageMetaTags.Add(new ImageParsedMetadataTagModel()
        {
            Software = SoftwareName,
            Name = "Prompt",
            Value = description,
            Source = exifDirName
        });
        imageMetaTags.Add(new ImageParsedMetadataTagModel()
        {
            Software = SoftwareName,
            Name = "Model",
            Value = model,
            Source = exifDirName
        });


        return true;
    }
}