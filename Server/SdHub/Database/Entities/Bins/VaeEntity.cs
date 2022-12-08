using System.Collections.Generic;
using SdHub.Database.Entities.Tags;
using SdHub.Models.Bins;

namespace SdHub.Database.Entities.Bins;

public class VaeEntity
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? About { get; set; }
    public SdVersion SdVersion { get; set; }
    public List<VaeVersionEntity>? Versions { get; set; }
    public List<VaeTagEntity>? VaeTags { get; set; }
}