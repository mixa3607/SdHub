using System.Collections.Generic;
using SdHub.Models.Files;

namespace SdHub.Models.Bins;

public class EmbeddingVersionModel
{
    public long Id { get; set; }

    public long EmbeddingId { get; set; }
    public EmbeddingModel? Embedding { get; set; }

    public FileModel? File { get; set; }
    public string? Version { get; set; }
    public string? About { get; set; }
    public string? SourceLink { get; set; }

    public IReadOnlyList<string> KnownNames { get; set; } = new List<string>();
}