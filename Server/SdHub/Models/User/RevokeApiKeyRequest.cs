using FluentValidation;

namespace SdHub.Models.User;

public class RevokeApiKeyRequest
{
    public required string Name { get; set; }

    public class Validator : AbstractValidator<RevokeApiKeyRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}