namespace SdHub.Database.Entities.Images;

public class ImageParsedMetadataTagEntity
{
    public long Id { get; set; }

    public string? Software { get; set; }
    public string? Source { get; set; }
    public string? Name { get; set; }
    public string? Value { get; set; }

    public long MetadataId { get; set; }
    public ImageParsedMetadataEntity? Metadata { get; set; }
}