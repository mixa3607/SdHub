using System.Collections.Generic;

namespace SdHub.Database.Entities.Images;

public class ImageParsedMetadataEntity
{
    public long ImageId { get; set; }
    public ImageEntity? Image { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public IReadOnlyList<ImageParsedMetadataTagEntity>? Tags { get; set; }
}