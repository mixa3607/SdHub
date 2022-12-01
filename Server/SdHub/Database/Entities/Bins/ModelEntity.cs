using System.Collections.Generic;
using SdHub.Models.Bins;

namespace SdHub.Database.Entities.Bins;

public class ModelEntity
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? About { get; set; }
    public SdVersion SdVersion { get; set; }
    public string? Author { get; set; }

    public List<ModelVersionEntity>? Versions { get; set; }
}