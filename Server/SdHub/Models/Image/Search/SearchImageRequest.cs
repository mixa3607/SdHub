using FluentValidation;
using System;
using System.Collections.Generic;

namespace SdHub.Models.Image;

public class SearchImageRequest
{
    public string? SearchText { get; set; }
    public string? Owner { get; set; }
    public string? Album { get; set; }

    public IReadOnlyList<SearchImageInFieldType> Fields { get; set; } = Array.Empty<SearchImageInFieldType>();
    public IReadOnlyList<string> Softwares { get; set; } = Array.Empty<string>();
    public bool AlsoFromGrids { get; set; }
    public bool OnlyFromRegisteredUsers { get; set; }
    public bool SearchAsRegexp { get; set; }

    public SearchImageOrderByFieldType OrderByField { get; set; }
    public SearchImageOrderByType OrderBy { get; set; }

    public int Skip { get; set; }
    public int Take { get; set; } = 20;

    public class Validator : AbstractValidator<SearchImageRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Softwares).NotNull();
            RuleFor(x => x.Fields).ForEach(x => x.IsInEnum()).NotNull();
            RuleFor(x => x.OrderBy).IsInEnum();
            RuleFor(x => x.OrderByField).IsInEnum();
            RuleFor(x => x.Take).NotEmpty().LessThanOrEqualTo(100);
        }
    }
}