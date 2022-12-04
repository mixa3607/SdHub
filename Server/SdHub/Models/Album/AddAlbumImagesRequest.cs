using System;
using System.Collections.Generic;
using FluentValidation;

namespace SdHub.Models.Album;

public class AddAlbumImagesRequest
{
    public string? AlbumShortToken { get; set; }
    public IReadOnlyList<string> Images { get; set; } = Array.Empty<string>();

    public class Validator : AbstractValidator<AddAlbumImagesRequest>
    {
        public Validator()
        {
            RuleFor(x => x.AlbumShortToken).NotEmpty();
        }
    }
}