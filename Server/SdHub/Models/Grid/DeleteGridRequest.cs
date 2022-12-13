using FluentValidation;

namespace SdHub.Models.Grid;

public class DeleteGridRequest
{
    public string? ShortToken { get; set; }

    public class Validator : AbstractValidator<DeleteGridRequest>
    {
        public Validator()
        {
            RuleFor(x => x.ShortToken).NotEmpty();
        }
    }
}