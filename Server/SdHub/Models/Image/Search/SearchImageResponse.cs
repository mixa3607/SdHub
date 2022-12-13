using System;
using System.Collections.Generic;

namespace SdHub.Models.Image;

public class SearchImageResponse
{
    public IReadOnlyList<ImageModel> Images { get; set; } = Array.Empty<ImageModel>();
    public int Total { get; set; }
}