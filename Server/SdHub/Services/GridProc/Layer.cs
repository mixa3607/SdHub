using System;
using System.Collections.Generic;

namespace SdHub.Services.GridProc;

public class Layer
{
    public int ZId { get; set; }
    public string? LayerDir { get; set; }
    public IReadOnlyList<LayerConverts> ConvertsList { get; set; } = Array.Empty<LayerConverts>();
    public IReadOnlyList<LayerRow> Rows { get; set; } = Array.Empty<LayerRow>();
}