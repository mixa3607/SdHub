using FluentValidation;

namespace SdHub.Models.User;

public class GetMeRequestValidator : AbstractValidator<GetMeRequest>
{
    public GetMeRequestValidator()
    {
    }
}
