using FluentValidation;

namespace SdHub.Models.Bins;

public class EditModelRequest
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? About { get; set; }
    public SdVersion? SdVersion { get; set; }

    public class Validator : AbstractValidator<EditModelRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.SdVersion).IsInEnum().When(x => x.SdVersion != null);
        }
    }
}