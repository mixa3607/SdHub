using System;
using SdHub.Models.Files;

namespace SdHub.Models.Album;

public class AlbumModel
{
    public string? ShortToken { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

    public string? Name { get; set; }
    public string? Description { get; set; }

    public UserSimpleModel? Owner { get; set; }

    public FileModel? ThumbImage { get; set; }
}