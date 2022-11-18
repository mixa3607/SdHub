using SdHub.Database.Entities;
using SdHub.Database.Entities.Files;
using SdHub.Database.Entities.Images;
using System;

namespace SdHub.Models;

public class ImageModel
{
    public ImageOwnerModel? Owner { get; set; }
    
    public FileModel? OriginalImage { get; set; }
    
    public FileModel? ThumbImage { get; set; }
    public string? ShortUrl { get; set; }
    public string? ShortToken { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

    public string? Name { get; set; }
    public string? Description { get; set; }

    public ImageParsedMetadataModel? ParsedMetadata { get; set; }
}