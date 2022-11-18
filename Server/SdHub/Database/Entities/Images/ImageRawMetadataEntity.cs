using System;
using System.Collections.Generic;
using SdHub.Services.FileProc.Metadata;

namespace SdHub.Database.Entities.Images;

public class ImageRawMetadataEntity
{
    public long ImageId { get; set; }
    public ImageEntity? Image { get; set; }

    public IReadOnlyList<ImageMetadataDirectory> Directories { get; set; } = Array.Empty<ImageMetadataDirectory>();
}