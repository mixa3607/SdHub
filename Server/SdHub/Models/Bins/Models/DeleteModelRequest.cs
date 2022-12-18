using FluentValidation;

namespace SdHub.Models.Bins;

public class DeleteModelRequest
{
    public long Id { get; set; }

    public class Validator : AbstractValidator<DeleteModelRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}