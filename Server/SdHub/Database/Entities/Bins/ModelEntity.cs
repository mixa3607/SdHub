using System.Collections.Generic;

namespace SdHub.Database.Entities.Bins;

public class ModelEntity
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string? About { get; set; }

    public List<GenerationSampleEntity>? Samples { get; set; }
    public List<ModelFileEntity>? Files { get; set; }
}