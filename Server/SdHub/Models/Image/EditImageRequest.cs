using FluentValidation;

namespace SdHub.Models.Image;

public class EditImageRequest
{
    public string? ManageToken { get; set; }

    public EditImageModel? Image { get; set; }

    public class Validator : AbstractValidator<EditImageRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Image).NotEmpty();
        }
    }
}