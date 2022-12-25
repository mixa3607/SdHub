using System.Collections.Generic;

namespace SdHub.Database.Entities.Bins;

public class ModelVersionEntity
{
    public long Id { get; set; }

    public long ModelId { get; set; }
    public ModelEntity? Model { get; set; }

    public string? Version { get; set; }
    public string? About { get; set; }

    public List<string> KnownNames { get; set; } = new List<string>();

    public List<GenerationSampleEntity>? Samples { get; set; }
    public List<ModelVersionFileEntity>? Files { get; set; }
}