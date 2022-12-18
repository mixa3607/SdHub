using System.Collections.Generic;
using SdHub.Database.Entities.Files;

namespace SdHub.Database.Entities.Bins;

public class ModelVersionEntity
{
    public long Id { get; set; }

    public long ModelId { get; set; }
    public ModelEntity? Model { get; set; }

    public string? HashV1 { get; set; }

    public long? CkptFileId { get; set; }
    public FileEntity? CkptFile { get; set; }

    public string? Version { get; set; }
    public string? About { get; set; }

    public List<string> KnownNames { get; set; } = new List<string>();

    public List<GenerationSampleEntity>? Samples { get; set; }
}