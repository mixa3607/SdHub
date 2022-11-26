using System;
using System.Collections.Generic;

namespace SdHub.Models.Image;

public class SearchImageRequest
{
    public string? SearchText { get; set; }

    //public IReadOnlyList<string> Tags { get; set; } = Array.Empty<string>();
    public IReadOnlyList<SearchImageInFieldType> Fields { get; set; } = Array.Empty<SearchImageInFieldType>();
    public IReadOnlyList<string> Softwares { get; set; } = Array.Empty<string>();
    public bool AlsoFromGrids { get; set; }
    public bool OnlyFromRegisteredUsers { get; set; }
    public bool SearchAsRegexp { get; set; }

    public SearchImageOrderByFieldType OrderByField { get; set; }
    public SearchImageOrderByType OrderBy { get; set; }

    public int Skip { get; set; }
    public int Take { get; set; } = 20;
}