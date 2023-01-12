using FluentValidation;

namespace SdHub.Models.Samples;

public class DeleteGenerationSampleRequest
{
    public long Id { get; set; }

    public class Validator : AbstractValidator<DeleteGenerationSampleRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}