using System;
using System.Collections.Generic;

namespace SdHub.Services.GridProc;

public class LayerRow
{
    public IReadOnlyList<string> Images { get; set; } = Array.Empty<string>();
}