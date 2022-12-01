using FluentValidation;

namespace SdHub.Models.User;

public class EditUserRequestValidator : AbstractValidator<EditUserRequest>
{
    public EditUserRequestValidator()
    {
        RuleFor(x => x.Login).NotEmpty();
        RuleFor(x => x.About).MaximumLength(4000);
    }
}