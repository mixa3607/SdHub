using Newtonsoft.Json;

namespace SdHub.Storage.Local;

public class LocalStorageSettings : IStorageSettings
{
    public string? PhysicalRoot { get; set; }
    public string? VirtualRoot { get; set; }

    public string? TempPath { get; set; }

    public bool Disabled { get; set; }

    public string Save() => JsonConvert.SerializeObject(this);
    public void Load(string json) => JsonConvert.PopulateObject(json, this);
}