namespace SdHub.Database.Entities.Files;

public class DirectoryEntity
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public long Size { get; set; }

    public string? StorageName { get; set; }
    public FileStorageEntity? Storage { get; set; }
    public string? PathOnStorage { get; set; }
}