﻿using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace SdHub.Models.Upload;

public class UploadGridRequest
{
    public string? AlbumShortToken { get; set; }
    public IFormFile? File { get; set; }

    public int XTiles { get; set; }
    public int YTiles { get; set; }

    public string? XValues { get; set; }
    public string? YValues { get; set; }

    public class Validator : AbstractValidator<UploadGridRequest>
    {
        public Validator()
        {
            RuleFor(x => x.File).NotEmpty();

            RuleFor(x => x.XTiles).NotEmpty().GreaterThan(0);
            RuleFor(x => x.XValues).NotEmpty();

            RuleFor(x => x.YTiles).NotEmpty().GreaterThan(0);
            RuleFor(x => x.YValues).NotEmpty();
        }
    }
}