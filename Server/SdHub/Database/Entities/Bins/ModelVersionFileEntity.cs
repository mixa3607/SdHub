using SdHub.Database.Entities.Files;
using SdHub.Models.Bins;

namespace SdHub.Database.Entities.Bins;

public class ModelVersionFileEntity
{
    public long ModelVersionId { get; set; }
    public ModelVersionEntity? ModelVersion { get; set; }

    public long FileId { get; set; }
    public FileEntity? File { get; set; }

    public ModelVersionFileType Type { get; set; }

    public string? ModelHashV1 { get; set; }
}