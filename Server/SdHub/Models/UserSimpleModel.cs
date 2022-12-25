using SdHub.Database.Entities.Images;
using System;

namespace SdHub.Models;

public class UserSimpleModel
{
    public Guid Guid { get; set; }
    public string? Login { get; set; }
    public bool IsAnonymous { get; set; }
    public string? LoginNormalized { get; set; }
}