using SdHub.Models.Image;

namespace SdHub.Models.Bins;

public class GenerationSampleModel
{
    public long Id { get; set; }

    public ImageModel? Image { get; set; }

    public long? ModelVersionId { get; set; }
    public ModelVersionModel? ModelVersion { get; set; }

    public long? HypernetVersionId { get; set; }
    public HypernetVersionModel? HypernetVersion { get; set; }

    public long? VaeVersionId { get; set; }
    public VaeVersionModel? VaeVersion { get; set; }

    public long? EmbeddingVersionId { get; set; }
    public EmbeddingVersionModel? EmbeddingVersion { get; set; }
}