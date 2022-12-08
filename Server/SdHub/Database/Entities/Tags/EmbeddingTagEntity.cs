using SdHub.Database.Entities.Bins;

namespace SdHub.Database.Entities.Tags;

public class EmbeddingTagEntity
{
    public long TagId { get; set; }
    public TagEntity? Tag { get; set; }
    public long EmbeddingId { get; set; }
    public EmbeddingEntity? Embedding { get; set; }
}