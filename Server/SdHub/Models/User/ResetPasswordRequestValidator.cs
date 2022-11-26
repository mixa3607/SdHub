using FluentValidation;

namespace SdHub.Models.User;

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Login).NotEmpty();
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.NewPassword).Matches(@"^(.{8,})$").WithMessage("Min 8 chars");
    }
}
