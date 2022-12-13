using FluentValidation;
using Reinforced.Typings.Attributes;

namespace SdHub.Models.Grid;

public class EditGridRequest
{
    public string? ShortToken { get; set; }

    [TsProperty(ForceNullable = true)]
    public string? Name { get; set; }

    [TsProperty(ForceNullable = true)]
    public string? Description { get; set; }

    public class Validator : AbstractValidator<EditGridRequest>
    {
        public Validator()
        {
            RuleFor(x => x.ShortToken).NotEmpty();
        }
    }
}