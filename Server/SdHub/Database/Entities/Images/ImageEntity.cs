using System;
using System.Collections.Generic;
using SdHub.Database.Entities.Albums;
using SdHub.Database.Entities.Bins;
using SdHub.Database.Entities.Files;
using SdHub.Database.Entities.Users;
using SdHub.Models.Album;

namespace SdHub.Database.Entities.Images;

/*public class TagForImageEntity
{
    public long Id { get; set; }
    public string? Value { get; set; }

    public List<ImageTagEntity>? ImageTags { get; set; }
}

public class ImageTagEntity
{
    public long ImageId { get; set; }
    public ImageEntity? Image { get; set; }

    public long TagId { get; set; }
    public TagForImageEntity? Tag { get; set; }
}*/

public class ImageEntity
{
    public long Id { get; set; }
    
    public long OwnerId { get; set; }
    public UserEntity? Owner { get; set; }

    public long UploaderId { get; set; }
    public ImageUploaderEntity? Uploader { get; set; }

    public long OriginalImageId { get; set; }
    public FileEntity? OriginalImage { get; set; }

    public long? ThumbImageId { get; set; }
    public FileEntity? ThumbImage { get; set; }

    public long? CompressedImageId { get; set; }
    public FileEntity? CompressedImage { get; set; }

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

    //public List<ImageTagEntity>? ImageTags { get; set; }
    public List<AlbumImageEntity>? AlbumImages { get; set; }
    public List<GenerationSampleEntity>? GenSamples { get; set; }
}