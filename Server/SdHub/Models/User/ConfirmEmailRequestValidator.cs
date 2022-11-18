using FluentValidation;

namespace SdHub.Models.User;

public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
{
    public ConfirmEmailRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty();
    }
}
