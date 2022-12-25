using System;
using System.Collections.Generic;
using SdHub.Models.Files;

namespace SdHub.Models.Grid;

public class GridModel
{
    public long Id { get; set; }

    public string? ShortUrl { get; set; }
    public string? ShortToken { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

    public string? Name { get; set; }
    public string? Description { get; set; }

    public UserSimpleModel? Owner { get; set; }

    public List<GridImageModel>? GridImages { get; set; }

    public int XTiles { get; set; }
    public int YTiles { get; set; }

    public List<string> XValues { get; set; } = new List<string>();
    public List<string> YValues { get; set; } = new List<string>();

    public int MinLayer { get; set; }
    public int MaxLayer { get; set; }

    public FileModel? ThumbImage { get; set; }
    public DirectoryModel? LayersDirectory { get; set; }
}