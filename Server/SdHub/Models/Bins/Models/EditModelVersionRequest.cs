using System.Collections.Generic;
using FluentValidation;

namespace SdHub.Models.Bins;

public class EditModelVersionRequest
{
    public long ModelId { get; set; }
    public long VersionId { get; set; }

    public string? Version { get; set; }
    public string? About { get; set; }
    public List<string>? KnownNames { get; set; }
    public Dictionary<string, ModelVersionFileType>? Files { get; set; }

    public class Validator : AbstractValidator<EditModelVersionRequest>
    {
        public Validator()
        {
            RuleFor(x => x.ModelId).NotEmpty();
            RuleFor(x => x.VersionId).NotEmpty();
        }
    }
}