using System;
using System.Collections.Generic;
using SdHub.Database.Entities.Albums;
using SdHub.Database.Entities.Files;
using SdHub.Database.Entities.Users;

namespace SdHub.Database.Entities.Grids;

public class GridEntity
{
    public long Id { get; set; }

    public string? ShortToken { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

    public string? Name { get; set; }
    public string? Description { get; set; }

    public long OwnerId { get; set; }
    public UserEntity? Owner { get; set; }

    public List<GridImageEntity>? GridImages { get; set; }

    public int XTiles { get; set; }
    public int YTiles { get; set; }

    public List<string> XValues { get; set; } = new List<string>();
    public List<string> YValues { get; set; } = new List<string>();

    public int MinLayer { get; set; }
    public int MaxLayer { get; set; }

    public long? ThumbImageId { get; set; }
    public FileEntity? ThumbImage { get; set; }

    public long? LayersDirectoryId { get; set; }
    public DirectoryEntity? LayersDirectory { get; set; }

    public List<AlbumImageEntity>? AlbumImages { get; set; }
}