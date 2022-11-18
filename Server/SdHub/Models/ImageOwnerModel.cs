using System;

namespace SdHub.Models;

public class ImageOwnerModel
{
    public Guid Guid { get; set; } = Guid.NewGuid();
    public string? Login { get; set; }
    public bool IsAnonymous { get; set; }
}