using FluentValidation;

namespace SdHub.Models.User;

public class SendEmailConfirmationEmailRequestValidator : AbstractValidator<SendEmailConfirmationEmailRequest>
{
    public SendEmailConfirmationEmailRequestValidator()
    {
        //RuleFor(x => x.CaptchaCode).NotEmpty();
        RuleFor(x => x.CaptchaType).IsInEnum();
        RuleFor(x => x.Login).NotEmpty();
    }
}