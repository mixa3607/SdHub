using System;
using System.Collections.Generic;

namespace SdHub.Models.Bins;

public class SearchModelResponse
{
    public IReadOnlyList<ModelModel> Models { get; set; } = Array.Empty<ModelModel>();
    public int Total { get; set; }
}