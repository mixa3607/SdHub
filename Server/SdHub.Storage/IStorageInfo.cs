namespace SdHub.Storage;

public interface IStorageInfo
{
    public string? Name { get; }
    public int Version { get; }
    public string? BaseUrl { get; }
}