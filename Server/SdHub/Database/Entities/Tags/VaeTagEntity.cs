using SdHub.Database.Entities.Bins;

namespace SdHub.Database.Entities.Tags;

public class VaeTagEntity
{
    public long TagId { get; set; }
    public TagEntity? Tag { get; set; }
    public long VaeId { get; set; }
    public VaeEntity? Vae { get; set; }
}