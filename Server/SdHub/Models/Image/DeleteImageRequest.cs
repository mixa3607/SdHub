using Reinforced.Typings.Attributes;

namespace SdHub.Models.Image;

public class DeleteImageRequest
{
    public string? ShortToken { get; set; }

    [TsProperty(ForceNullable = true)]
    public string? ManageToken { get; set; }
}