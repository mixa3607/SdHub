using System.Collections.Generic;
using SdHub.Models;
using SdHub.Services.FileProc.Metadata;

namespace SdHub.Services.FileProc.Detectors;

public interface IMetadataSoftwareDetector
{
    string SoftwareName { get; }
    bool TryExtract(List<ImageParsedMetadataTagModel> imageMetaTags, IReadOnlyList<ImageMetadataDirectory> meta);
}