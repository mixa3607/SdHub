using FluentValidation;

namespace SdHub.Models.Bins;

public class DeleteModelVersionRequest
{
    public long ModelId { get; set; }
    public long VersionId { get; set; }

    public class Validator : AbstractValidator<DeleteModelVersionRequest>
    {
        public Validator()
        {
            RuleFor(x => x.ModelId).NotEmpty();
            RuleFor(x => x.VersionId).NotEmpty();
        }
    }
}