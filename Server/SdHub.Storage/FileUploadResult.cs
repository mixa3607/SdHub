namespace SdHub.Storage;

public class FileUploadResult
{
    public FileUploadResult(string pathOnStorage, long size, string storageName)
    {
        PathOnStorage = pathOnStorage;
        StorageName = storageName;
        Size = size;
    }

    public string StorageName { get; init; }
    public string PathOnStorage { get; init; }
    public long Size { get; init; }
}