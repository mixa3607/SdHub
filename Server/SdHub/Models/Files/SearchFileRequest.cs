using FluentValidation;

namespace SdHub.Models.Files;

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