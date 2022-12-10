using System;
using System.Collections.Generic;

namespace SdHub.Services.GridProc;

public class LayerConverts
{
    public int XTiles { get; set; } = -1;
    public int YTiles { get; set; } = -1;

    public string Geometry = "100%x";
    public IReadOnlyList<string?> Sources { get; set; } = Array.Empty<string>();
    public string? Destination { get; set; }
}