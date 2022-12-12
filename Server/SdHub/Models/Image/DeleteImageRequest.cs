using Reinforced.Typings.Attributes;

namespace SdHub.Models.Image;

public class DeleteImageRequest
{
    public string? ShortToken { get; set; }

    [TsProperty(Type = "string | null")]
    public string? ManageToken { get; set; }
}