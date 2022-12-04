using System;
using System.Collections.Generic;
using FluentValidation;

namespace SdHub.Models.Album;

public class DeleteAlbumImagesRequest
{
    public string? AlbumShortToken { get; set; }
    public IReadOnlyList<string> Images { get; set; } = Array.Empty<string>();

    public class Validator : AbstractValidator<DeleteAlbumImagesRequest>
    {
        public Validator()
        {
            RuleFor(x => x.AlbumShortToken).NotEmpty();
        }
    }
}