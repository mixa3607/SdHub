using FluentValidation;

namespace SdHub.Models.User;

public class RevokeApiKeyRequest
{

    public string? Token { get; set; }
    public class Validator : AbstractValidator<RevokeApiKeyRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Token).NotEmpty();
        }
    }
}