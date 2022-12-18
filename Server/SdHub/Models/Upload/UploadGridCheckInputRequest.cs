using FluentValidation;

namespace SdHub.Models.Upload;

public class UploadGridCheckInputRequest
{
    public int XTiles { get; set; }
    public int YTiles { get; set; }

    public string? XValues { get; set; }
    public string? YValues { get; set; }

    public class Validator : AbstractValidator<UploadGridCheckInputRequest>
    {
        public Validator()
        {
            RuleFor(x => x.XTiles).NotEmpty().GreaterThan(0);
            RuleFor(x => x.XValues).NotEmpty();

            RuleFor(x => x.YTiles).NotEmpty().GreaterThan(0);
            RuleFor(x => x.YValues).NotEmpty();
        }
    }
}