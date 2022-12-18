using FluentValidation;

namespace SdHub.Models.Bins;

public class GetModelRequest
{
    public long Id { get; set; }

    public class Validator : AbstractValidator<GetModelRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}