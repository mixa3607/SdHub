using FluentValidation;

namespace SdHub.Models.Image;

public class DeleteImageRequestValidator : AbstractValidator<DeleteImageRequest>
{
    public DeleteImageRequestValidator()
    {
        RuleFor(x => x.ShortToken).NotEmpty();
    }
}