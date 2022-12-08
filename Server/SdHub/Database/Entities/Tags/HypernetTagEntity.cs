using SdHub.Database.Entities.Bins;

namespace SdHub.Database.Entities.Tags;

public class HypernetTagEntity
{
    public long TagId { get; set; }
    public TagEntity? Tag { get; set; }
    public long HypernetId { get; set; }
    public HypernetEntity? Hypernet { get; set; }
}