using System;
using System.Collections.Generic;

namespace SdHub.Models.Album;

public class SearchAlbumResponse
{
    public IReadOnlyList<AlbumModel> Albums { get; set; } = Array.Empty<AlbumModel>();
    public int Total { get; set; }
}