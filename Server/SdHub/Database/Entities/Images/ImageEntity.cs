using System;
using SdHub.Database.Entities.Files;
using SdHub.Database.Entities.User;

namespace SdHub.Database.Entities.Images;

public class ImageEntity
{
    public long Id { get; set; }

    /// <summary>
    /// null for anon
    /// </summary>
    public long OwnerId { get; set; }

    public UserEntity? Owner { get; set; }

    public long UploaderId { get; set; }
    public ImageUploaderEntity? Uploader { get; set; }

    public long OriginalImageId { get; set; }
    public FileEntity? OriginalImage { get; set; }

    public long? ThumbImageId { get; set; }
    public FileEntity? ThumbImage { get; set; }

    /// <summary>
    /// base58 code
    /// </summary>
    public string? ShortToken { get; set; }
    public string? ManageToken { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

    public string? Name { get; set; }
    public string? Description { get; set; }

    public ImageParsedMetadataEntity? ParsedMetadata { get; set; }
    public ImageRawMetadataEntity? RawMetadata { get; set; }
}