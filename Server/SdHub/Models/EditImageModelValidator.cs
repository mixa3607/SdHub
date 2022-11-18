using FluentValidation;

namespace SdHub.Models;

public class EditImageModelValidator : AbstractValidator<EditImageModel>
{
    public EditImageModelValidator()
    {
        RuleFor(x => x.ShortToken).NotEmpty();
    }
}