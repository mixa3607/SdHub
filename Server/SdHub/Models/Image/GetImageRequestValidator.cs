using FluentValidation;

namespace SdHub.Models.Image;

public class GetImageRequestValidator : AbstractValidator<GetImageRequest>
{
    public GetImageRequestValidator()
    {
        RuleFor(x => x.ShortToken).NotEmpty();
    }
}
