using System;
using System.Collections.Generic;

namespace SdHub.Models.Grid;

public class SearchGridResponse
{
    public IReadOnlyList<GridModel> Grids { get; set; } = Array.Empty<GridModel>();
    public int Total { get; set; }
}