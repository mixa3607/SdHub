using SdHub.Models.Files;

namespace SdHub.Models.Bins;

public class ModelVersionFileModel
{
    public long ModelVersionId { get; set; }
    public FileModel? File { get; set; }
    public ModelFileType Type { get; set; }
    public string? ModelHashV1 { get; set; }
}