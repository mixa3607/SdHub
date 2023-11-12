using System.Collections.Generic;

namespace SdHub.Models.Bins.Hypernets;

public class HypernetModel
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? About { get; set; }

    public IReadOnlyList<HypernetVersionModel>? Versions { get; set; }
}