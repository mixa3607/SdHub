using FluentValidation;

namespace SdHub.Models.User;

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.CaptchaType).IsInEnum();
        RuleFor(x => x.CaptchaCode).NotEmpty();
        RuleFor(x => x.Email).EmailAddress().NotEmpty();
    }
}
