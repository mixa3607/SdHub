using FluentValidation;

namespace SdHub.Models.User;

public class LoginByRefreshTokenRequestValidator : AbstractValidator<LoginByRefreshTokenRequest>
{
    public LoginByRefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
