using System;
using System.Collections.Generic;

namespace SdHub.Models;

public class UserModel
{
    public Guid Guid { get; set; }
    public string? Login { get; set; }
    public string? LoginNormalized { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public IReadOnlyList<string>? Roles { get; set; } = Array.Empty<string>();
    public string? About { get; set; }
}