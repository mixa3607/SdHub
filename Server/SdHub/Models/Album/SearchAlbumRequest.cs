using System;
using System.Collections.Generic;
using FluentValidation;

namespace SdHub.Models.Album;

public class SearchAlbumRequest
{
    public string? SearchText { get; set; }
    public string? Owner { get; set; }

    //public IReadOnlyList<string> Tags { get; set; } = Array.Empty<string>();
    public IReadOnlyList<SearchAlbumInFieldType> Fields { get; set; } = Array.Empty<SearchAlbumInFieldType>();

    public SearchAlbumOrderByFieldType OrderByField { get; set; }
    public SearchAlbumOrderByType OrderBy { get; set; }

    public int Skip { get; set; }
    public int Take { get; set; } = 20;

    public class Validator : AbstractValidator<SearchAlbumRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Fields).ForEach(x => x.IsInEnum()).NotNull();
            RuleFor(x => x.OrderBy).IsInEnum();
            RuleFor(x => x.OrderByField).IsInEnum();
            RuleFor(x => x.Take).NotEmpty().LessThanOrEqualTo(100);
        }
    }
}