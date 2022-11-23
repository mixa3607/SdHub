using Newtonsoft.Json;

namespace SdHub.Storage.S3;

public class S3StorageSettings : IStorageSettings
{
    public string? Endpoint { get; set; }
    public string? Login { get; set; }
    public string? Password { get; set; }
    public bool WithSsl { get; set; } = true;
    public string? PolicyJson { get; set; }
    public string? BucketName { get; set; }

    public bool Disabled { get; set; }

    public string Save() => JsonConvert.SerializeObject(this);
    public void Load(string json) => JsonConvert.PopulateObject(json, this);
}