using System.Collections.Generic;
using SdHub.Database.Entities.Files;

namespace SdHub.Models.Bins;

public class ModelVersionModel
{
    public long Id { get; set; }

    public long ModelId { get; set; }
    public ModelModel? Model { get; set; }

    public FileModel? CkptFile { get; set; }
    public string? Version { get; set; }
    public string? About { get; set; }
    public string? SourceLink { get; set; }

    public IReadOnlyList<string> KnownNames { get; set; } = new List<string>();
}