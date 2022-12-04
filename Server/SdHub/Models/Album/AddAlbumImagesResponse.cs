using System;
using System.Collections.Generic;

namespace SdHub.Models.Album;

public class AddAlbumImagesResponse
{
    public IReadOnlyList<string> AddedImages { get; set; } = Array.Empty<string>();
}