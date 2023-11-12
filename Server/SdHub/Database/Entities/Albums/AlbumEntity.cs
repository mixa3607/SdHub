using SdHub.Database.Entities.Files;
using System;
using System.Collections.Generic;
using SdHub.Database.Entities.Users;

namespace SdHub.Database.Entities.Albums;

public class AlbumEntity : IEntityWithDeletingFlag
{
    public long Id { get; set; }

    public string? ShortToken { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }

    public long OwnerId { get; set; }
    public UserEntity? Owner { get; set; }

    public long? ThumbImageId { get; set; }
    public FileEntity? ThumbImage { get; set; }

    public List<AlbumImageEntity>? AlbumImages { get; set; }
}