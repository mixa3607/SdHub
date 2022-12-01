using System.Collections.Generic;

namespace SdHub.Models.Bins;

public class VaeModel
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? About { get; set; }
    public SdVersion SdVersion { get; set; }

    public IReadOnlyList<VaeVersionModel>? Versions { get; set; }
}