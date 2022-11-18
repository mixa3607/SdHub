using SdHub.Services.FileProc.Metadata;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace SdHub.Services.FileProc.Extractor;

public interface IImageMetadataExtractor
{
    IReadOnlyList<ImageMetadataDirectory> ExtractMetadata(Stream fileStream,
        CancellationToken ct = default);
}