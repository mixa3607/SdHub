using FluentValidation;

namespace SdHub.Models.Album;

public class CreateAlbumRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    public class Validator : AbstractValidator<CreateAlbumRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}