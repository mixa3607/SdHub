using FluentValidation;

namespace SdHub.Models.Image;

public class CheckManageTokenRequestValidator : AbstractValidator<CheckManageTokenRequest>
{
    public CheckManageTokenRequestValidator()
    {
        RuleFor(x => x.ShortToken).NotEmpty();
        RuleFor(x => x.ManageToken).NotEmpty();
    }
}