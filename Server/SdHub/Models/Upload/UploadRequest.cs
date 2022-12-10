using FluentValidation;
using Microsoft.AspNetCore.Http;
using SdHub.Models.Image;
using System;
using System.Collections.Generic;

namespace SdHub.Models.Upload;

public class UploadRequest
{
    public string? AlbumShortToken { get; set; }
    public List<IFormFile> Files { get; set; } = new List<IFormFile>();

    public class Validator : AbstractValidator<UploadRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Files).NotEmpty();
        }
    }
}