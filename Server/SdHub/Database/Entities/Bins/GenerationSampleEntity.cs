using SdHub.Database.Entities.Images;

namespace SdHub.Database.Entities.Bins;

public class GenerationSampleEntity
{
    public long Id { get; set; }

    public long ImageId { get; set; }
    public ImageEntity? Image { get; set; }

    public long? ModelId { get; set; }
    public ModelEntity? Model { get; set; }

    public long? HypernetId { get; set; }
    public HypernetEntity? Hypernet { get; set; }

    public long? VaeId { get; set; }
    public VaeEntity? Vae { get; set; }

    public long? EmbeddingId { get; set; }
    public EmbeddingEntity? Embedding { get; set; }
}