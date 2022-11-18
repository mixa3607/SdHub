namespace SdHub.Storage;

public interface IStorageSettings
{
    string Save();
    void Load(string json);
}