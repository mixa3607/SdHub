using System;
using System.Collections.Generic;
using FluentValidation;

namespace SdHub.Models.Grid;

public class SearchGridRequest
{
    public string? SearchText { get; set; }
    public string? Owner { get; set; }
    public string? Album { get; set; }

    public IReadOnlyList<SearchGridInFieldType> Fields { get; set; } = Array.Empty<SearchGridInFieldType>();
    public SearchGridOrderByFieldType OrderByField { get; set; }
    public SearchGridOrderByType OrderBy { get; set; }

    public int Skip { get; set; }
    public int Take { get; set; } = 20;

    public class Validator : AbstractValidator<SearchGridRequest>
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