using FluentValidation;

namespace SdHub.Models.User;

public class SendResetPasswordEmailRequestValidator : AbstractValidator<SendResetPasswordEmailRequest>
{
    public SendResetPasswordEmailRequestValidator()
    {
        RuleFor(x => x.CaptchaCode).NotEmpty();
        RuleFor(x => x.CaptchaType).IsInEnum();
        RuleFor(x => x.Login).NotEmpty();
    }
}