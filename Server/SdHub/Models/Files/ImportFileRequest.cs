using FluentValidation;

namespace SdHub.Models.Files;

public class ImportFileRequest
{
    public string? FileUrl { get; set; }

    public class Validator : AbstractValidator<ImportFileRequest>
    {
        public Validator()
        {
            RuleFor(x => x.FileUrl).NotEmpty();
        }
    }
}