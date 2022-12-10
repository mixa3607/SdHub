using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace SdHub.Models.Upload;

public class UploadGridRequest
{
    public string? AlbumShortToken { get; set; }
    public IFormFile? File { get; set; }

    public int XTiles { get; set; }
    public int YTiles { get; set; }

    public List<string> XValues { get; set; } = new List<string>();
    public List<string> YValues { get; set; } = new List<string>();

    public class Validator : AbstractValidator<UploadGridRequest>
    {
        public Validator()
        {
            RuleFor(x => x.File).NotEmpty();

            RuleFor(x => x.XTiles).NotEmpty().GreaterThan(0);
            RuleFor(x => x.XValues).NotEmpty().Must((r, x) => x.Count == r.XTiles);

            RuleFor(x => x.YTiles).NotEmpty().GreaterThan(0);
            RuleFor(x => x.YValues).NotEmpty().Must((r, x) => x.Count == r.YTiles);
        }
    }
}