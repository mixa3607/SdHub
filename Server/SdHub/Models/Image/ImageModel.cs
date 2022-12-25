using SdHub.Models.Files;
using System;

namespace SdHub.Models.Image;

public class ImageModel
{
    public UserSimpleModel? Owner { get; set; }

    public FileModel? OriginalImage { get; set; }
    public FileModel? CompressedImage { get; set; }

    public string? ShortUrl { get; set; }
    public string? ShortToken { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

    public string? Name { get; set; }
    public string? Description { get; set; }

    public ImageParsedMetadataModel? ParsedMetadata { get; set; }
}