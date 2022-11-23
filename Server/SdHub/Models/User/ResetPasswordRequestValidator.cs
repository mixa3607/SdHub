using FluentValidation;

namespace SdHub.Models.User;

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Login).NotEmpty();
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.NewPassword).Matches(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$");
    }
}
