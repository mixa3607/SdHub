using System.Security.Cryptography.X509Certificates;

namespace SdHub.Storage;

public class FileSaveResult
{
    public FileSaveResult(string hash, string pathOnStorage, string storageName, long size, string extension, string mimeType, string name)
    {
        Hash = hash;
        PathOnStorage = pathOnStorage;
        StorageName = storageName;
        Size = size;
        Extension = extension;
        MimeType = mimeType;
        Name = name;
    }

    public string StorageName { get; init; }

    public string Hash { get; init; }
    public string PathOnStorage { get; init; }
    public long Size { get; init; }
    public string Extension { get; set; }
    public string MimeType { get; set; }
    public string Name { get; set; }
}

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