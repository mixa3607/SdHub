using System.Collections.Generic;

namespace SdHub.Services.FileProc.Metadata;

public class ImageMetadataDirectory
{
    public required string Type { get; set; }
    public required IReadOnlyList<ImageMetadataTag> Tags { get; set; }

    public override string ToString()
    {
        return $"{Type}[{Tags.Count}]";
    }
}