using System.Collections.Generic;

namespace SdHub.Database.Entities.Users;

public class UserPlanEntity
{
    public long Id { get; set; }
    public string? Name { get; set; }

    public int ImagesPerHour { get; set; }
    public bool OnlyWithMetadata { get; set; }
    public int MaxArchiveSizeUpload { get; set; }
    public int MaxImageSizeUpload { get; set; }
    public int ImagesPerUpload { get; set; }
}