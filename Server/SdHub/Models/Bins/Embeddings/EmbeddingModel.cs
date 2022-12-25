using System.Collections.Generic;

namespace SdHub.Models.Bins.Embeddings;

public class EmbeddingModel
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? About { get; set; }
    public string? Trigger { get; set; }
    public SdVersion SdVersion { get; set; }

    public IReadOnlyList<EmbeddingVersionModel>? Versions { get; set; }
}