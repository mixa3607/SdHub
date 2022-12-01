using System.Collections.Generic;
using SdHub.Models.Files;

namespace SdHub.Models.Bins;

public class VaeVersionModel
{
    public long Id { get; set; }

    public long VaeId { get; set; }
    public VaeModel? Vae { get; set; }

    public FileModel? File { get; set; }
    public string? Version { get; set; }
    public string? About { get; set; }
    public string? SourceLink { get; set; }

    public IReadOnlyList<string> KnownNames { get; set; } = new List<string>();
}