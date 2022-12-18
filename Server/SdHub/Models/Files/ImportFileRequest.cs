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

public class SearchFileRequest
{
    public string? SearchText { get; set; }
    public string? Storage { get; set; }
    public int Skip { get; set; }
    public int Take { get; set; } = 20;

    public class Validator : AbstractValidator<SearchFileRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Take).NotEmpty().LessThanOrEqualTo(100);
        }
    }
}