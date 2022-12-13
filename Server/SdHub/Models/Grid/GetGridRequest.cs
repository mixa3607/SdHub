using FluentValidation;

namespace SdHub.Models.Grid;

public class GetGridRequest
{
    public string? ShortToken { get; set; }
    public class Validator : AbstractValidator<GetGridRequest>
    {
        public Validator()
        {
            RuleFor(x => x.ShortToken).NotEmpty();
        }
    }
}