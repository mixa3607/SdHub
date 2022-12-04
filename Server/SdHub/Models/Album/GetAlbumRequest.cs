using FluentValidation;

namespace SdHub.Models.Album;

public class GetAlbumRequest
{
    public string? ShortToken { get; set; }

    public class Validator : AbstractValidator<GetAlbumRequest>
    {
        public Validator()
        {
            RuleFor(x => x.ShortToken).NotEmpty();
        }
    }
}