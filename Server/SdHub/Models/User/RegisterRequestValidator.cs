using FluentValidation;

namespace SdHub.Models.User;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Login).Matches("[A-z0-9]{4,20}");
        RuleFor(x => x.Email).EmailAddress().NotEmpty();
        RuleFor(x => x.Password).Matches(@"^(.{8,})$").WithMessage("Min 8 chars");
        RuleFor(x => x.CaptchaType).IsInEnum();
        RuleFor(x => x.CaptchaCode).NotEmpty();
    }
}
