using FluentValidation;
using SdHub.Models.Album;

namespace SdHub.Models.Bins;

public class CreateModelRequest
{
    public string? Name { get; set; }

    public class Validator : AbstractValidator<CreateAlbumRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}