using FluentValidation;

namespace SdHub.Models.User;

public class GetUserRequestValidator : AbstractValidator<GetUserRequest>
{
    public GetUserRequestValidator()
    {
        RuleFor(x => x.Login).NotEmpty();
    }
}