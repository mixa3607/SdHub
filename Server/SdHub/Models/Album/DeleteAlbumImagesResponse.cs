using System;
using System.Collections.Generic;

namespace SdHub.Models.Album;

public class DeleteAlbumImagesResponse
{
    public IReadOnlyList<string> DeletedImages { get; set; } = Array.Empty<string>();
}