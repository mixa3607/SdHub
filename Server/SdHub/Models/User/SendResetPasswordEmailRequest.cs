using FluentValidation;
using SdHub.Models.Enums;

namespace SdHub.Models.User;

public class SendResetPasswordEmailRequest
{
    public required string Login { get; set; }

    public CaptchaType CaptchaType { get; set; }
    public string? CaptchaCode { get; set; }

    public class Validator : AbstractValidator<SendResetPasswordEmailRequest>
    {
        public Validator()
        {
            //RuleFor(x => x.CaptchaCode).NotEmpty();
            RuleFor(x => x.CaptchaType).IsInEnum();
            RuleFor(x => x.Login).NotEmpty();
        }
    }
}