using FluentValidation;

namespace SdHub.Models.Image;

public class EditImageModelValidator : AbstractValidator<EditImageModel>
{
    public EditImageModelValidator()
    {
        RuleFor(x => x.ShortToken).NotEmpty();
    }
}