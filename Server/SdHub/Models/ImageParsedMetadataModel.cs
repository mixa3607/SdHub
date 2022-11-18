using System;
using System.Collections.Generic;

namespace SdHub.Models;

public class ImageParsedMetadataModel
{
    public int Width { get; set; }
    public int Height { get; set; }
    public IReadOnlyList<ImageParsedMetadataTagModel> Tags { get; set; } = Array.Empty<ImageParsedMetadataTagModel>();
}