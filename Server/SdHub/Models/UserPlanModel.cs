namespace SdHub.Models;

public class UserPlanModel
{
    public string? Name { get; set; }

    public int ImagesPerHour { get; set; }
    public bool OnlyWithMetadata { get; set; }
    public int MaxArchiveSizeUpload { get; set; }
    public int MaxImageSizeUpload { get; set; }
    public int ImagesPerUpload { get; set; }
}