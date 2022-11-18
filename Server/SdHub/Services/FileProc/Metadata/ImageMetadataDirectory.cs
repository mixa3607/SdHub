using System;
using System.Collections.Generic;

namespace SdHub.Services.FileProc.Metadata;

public class ImageMetadataDirectory
{
    public string? Type { get; set; }
    public IReadOnlyList<ImageMetadataTag> Tags { get; set; } = Array.Empty<ImageMetadataTag>();

    public override string ToString()
    {
        return $"{Type}[{Tags.Count}]";
    }
}