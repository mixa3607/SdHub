namespace SdHub.Storage;

public enum FileStorageBackendType : byte
{
    Unknown = 0,
    LocalDir,
    S3,
}