using SdHub.Database.Entities.Bins;

namespace SdHub.Database.Entities.Tags;

public class ModelTagEntity
{
    public long TagId { get; set; }
    public TagEntity? Tag { get; set; }
    public long ModelId { get; set; }
    public ModelEntity? Model { get; set; }
}