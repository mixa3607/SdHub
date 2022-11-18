using FluentValidation;

namespace SdHub.Models.Image;

public class EditImageRequestValidator : AbstractValidator<EditImageRequest>
{
    public EditImageRequestValidator()
    {
        RuleFor(x => x.Image).NotEmpty();
    }
}