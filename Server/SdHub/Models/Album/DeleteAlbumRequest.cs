using FluentValidation;

namespace SdHub.Models.Album;

public class DeleteAlbumRequest
{
    public string? ShortToken { get; set; }

    public class Validator : AbstractValidator<DeleteAlbumRequest>
    {
        public Validator()
        {
            RuleFor(x => x.ShortToken).NotEmpty();
        }
    }
}