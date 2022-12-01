using System.Collections.Generic;

namespace SdHub.Models.Bins;

public class ModelModel
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? About { get; set; }
    public SdVersion SdVersion { get; set; }

    public IReadOnlyList<ModelVersionModel>? Versions { get; set; }
}