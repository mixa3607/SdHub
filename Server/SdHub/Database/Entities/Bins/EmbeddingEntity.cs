using System.Collections.Generic;
using SdHub.Database.Entities.Tags;
using SdHub.Models.Bins;

namespace SdHub.Database.Entities.Bins;

public class EmbeddingEntity
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? About { get; set; }
    public string? Trigger { get; set; }
    public SdVersion SdVersion { get; set; }

    public List<EmbeddingVersionEntity>? Versions { get; set; }
    public List<EmbeddingTagEntity>? EmbeddingTags { get; set; }
}