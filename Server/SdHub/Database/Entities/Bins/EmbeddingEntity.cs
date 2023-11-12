using SdHub.Database.Entities.Files;

namespace SdHub.Database.Entities.Bins;

public class EmbeddingEntity
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? About { get; set; }
    public string? Trigger { get; set; }
    public long FileId { get; set; }
    public FileEntity? File { get; set; }
}