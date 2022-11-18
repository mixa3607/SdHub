using FluentValidation;

namespace SdHub.Models.User;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.NewPassword).Matches(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$");
        RuleFor(x => x.Password).Matches(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$");
    }
}
