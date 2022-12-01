using SdHub.Database.Entities.Images;

namespace SdHub.Database.Entities.Bins;

public class GenerationSampleEntity
{
    public long Id { get; set; }

    public long ImageId { get; set; }
    public ImageEntity? Image { get; set; }

    public long? ModelVersionId { get; set; }
    public ModelVersionEntity? ModelVersion { get; set; }

    public long? HypernetVersionId { get; set; }
    public HypernetVersionEntity? HypernetVersion { get; set; }

    public long? VaeVersionId { get; set; }
    public VaeVersionEntity? VaeVersion { get; set; }

    public long? EmbeddingVersionId { get; set; }
    public EmbeddingVersionEntity? EmbeddingVersion { get; set; }
}