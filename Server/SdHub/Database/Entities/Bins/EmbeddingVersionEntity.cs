using System.Collections.Generic;
using SdHub.Database.Entities.Files;

namespace SdHub.Database.Entities.Bins;

public class EmbeddingVersionEntity
{
    public long Id { get; set; }

    public long EmbeddingId { get; set; }
    public EmbeddingEntity? Embedding { get; set; }

    public long FileId { get; set; }
    public FileEntity? File { get; set; }
    public string? Version { get; set; }
    public string? About { get; set; }
    public string? SourceLink { get; set; }

    public List<string> KnownNames { get; set; } = new List<string>();
}