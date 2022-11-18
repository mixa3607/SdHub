namespace SdHub.Services.FileProc.Metadata;

public class ImageMetadataTag
{
    public ImageMetadataTag(int type, string name, ImageMetadataTagValue value)
    {
        Type = type;
        Name = name;
        Value = value;
    }

    public int Type { get; set; }
    public string Name { get; set; }
    public ImageMetadataTagValue Value { get; set; }

    public override string ToString()
    {
        return $"{Name} = {Value}";
    }
}