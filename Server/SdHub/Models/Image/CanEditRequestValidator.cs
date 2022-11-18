using FluentValidation;

namespace SdHub.Models.Image;

public class CanEditRequestValidator : AbstractValidator<CanEditRequest>
{
    public CanEditRequestValidator()
    {
        RuleFor(x => x.ShortToken).NotEmpty();
    }
}