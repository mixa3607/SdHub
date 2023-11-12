using FluentValidation;
using SdHub.Models.Album;

namespace SdHub.Models.Bins;

public class CreateModelRequest
{
    public required string Name { get; set; }

    public class Validator : AbstractValidator<CreateModelRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}