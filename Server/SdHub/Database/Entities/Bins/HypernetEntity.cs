using SdHub.Models.Bins;
using System.Collections.Generic;

namespace SdHub.Database.Entities.Bins;

public class HypernetEntity
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? About { get; set; }
    public SdVersion SdVersion { get; set; }
    public List<HypernetVersionEntity>? Versions { get; set; }
}