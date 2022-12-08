using System.Collections.Generic;

namespace SdHub.Database.Entities.Tags;

public class TagEntity
{
    public long Id { get; set; }
    public string? Value { get; set; }

    public List<EmbeddingTagEntity>? EmbeddingTags { get; set; }
    public List<VaeTagEntity>? VaeTags { get; set; }
    public List<HypernetTagEntity>? HypernetTags { get; set; }
    public List<ModelTagEntity>? ModelTags { get; set; }
}