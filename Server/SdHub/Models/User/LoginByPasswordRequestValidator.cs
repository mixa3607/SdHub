using FluentValidation;

namespace SdHub.Models.User;

public class LoginByPasswordRequestValidator : AbstractValidator<LoginByPasswordRequest>
{
    public LoginByPasswordRequestValidator()
    {
        RuleFor(x => x.CaptchaType).IsInEnum();
        RuleFor(x => x.CaptchaCode).NotEmpty();
        RuleFor(x => x.Login).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}
