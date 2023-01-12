﻿using System.Collections.Generic;
using SdHub.Models.Samples;

namespace SdHub.Models.Bins;

public class ModelVersionModel
{
    public long Id { get; set; }

    public long ModelId { get; set; }
    
    public int Order { get; set; }
    public string? Version { get; set; }
    public string? About { get; set; }

    public IReadOnlyList<string> KnownNames { get; set; } = new List<string>();
    public IReadOnlyList<GenerationSampleModel>? Samples { get; set; }
    public IReadOnlyList<ModelVersionFileModel>? Files { get; set; }
}