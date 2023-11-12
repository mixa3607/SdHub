using SdHub.Database.Entities.Files;
using SdHub.Models.Bins;

namespace SdHub.Database.Entities.Bins;

public class ModelFileEntity
{
    public long ModelId { get; set; }
    public ModelEntity? Model { get; set; }

    public long FileId { get; set; }
    public FileEntity? File { get; set; }

    public ModelFileType Type { get; set; }

    public string? ModelHashV1 { get; set; }
    public string? ModelHashV2 { get; set; }
}