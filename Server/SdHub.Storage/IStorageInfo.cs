namespace SdHub.Storage;

public interface IStorageInfo
{
    string? Name { get; }
    int Version { get; }
    string? BaseUrl { get; }
}