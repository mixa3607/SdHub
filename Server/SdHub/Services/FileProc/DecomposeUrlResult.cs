using SdHub.Storage;

namespace SdHub.Services.FileProc;

public class DecomposeUrlResult
{
    public DecomposeUrlResult(IFileStorage storage, string pathOnStorage)
    {
        Storage = storage;
        PathOnStorage = pathOnStorage;
    }

    public IFileStorage Storage { get; init; }
    public string PathOnStorage { get; init; }
}