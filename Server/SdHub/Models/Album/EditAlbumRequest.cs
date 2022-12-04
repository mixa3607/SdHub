using FluentValidation;

namespace SdHub.Models.Album;

public class EditAlbumRequest
{
    public string? ShortToken { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    public class Validator : AbstractValidator<EditAlbumRequest>
    {
        public Validator()
        {
            RuleFor(x => x.ShortToken).NotEmpty();
        }
    }
}