using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace SdHub.Models;

public class EditImageModel
{
    public string? ShortToken { get; set; }

    [TsProperty(ForceNullable = true)]
    public string? Name { get; set; }

    [TsProperty(ForceNullable = true)]
    public string? Description { get; set; }
}