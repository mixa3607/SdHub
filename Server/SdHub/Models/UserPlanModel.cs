namespace SdHub.Models;

public class UserPlanModel
{
    public string? Name { get; set; }

    public bool OnlyWithMetadata { get; set; }

    public int ImagesPerHour { get; set; }
    public long MaxImageSizeUpload { get; set; }
    public int ImagesPerUpload { get; set; }

    public int GridsPerHour { get; set; }
    public long MaxGridArchiveSizeUpload { get; set; }
    public int ImagesPerGrid { get; set; }
}