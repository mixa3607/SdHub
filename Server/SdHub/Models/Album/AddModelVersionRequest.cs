using FluentValidation;

namespace SdHub.Models.Album;

public class AddModelVersionRequest
{
    public long ModelId { get; set; }

    public class Validator : AbstractValidator<AddModelVersionRequest>
    {
        public Validator()
        {
            RuleFor(x => x.ModelId).NotEmpty();
        }
    }
}