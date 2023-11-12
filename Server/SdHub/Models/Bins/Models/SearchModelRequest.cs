using System;
using System.Collections.Generic;
using FluentValidation;

namespace SdHub.Models.Bins;

public class SearchModelRequest
{
    public string? SearchText { get; set; }

    public IReadOnlyList<SearchModelInFieldType> Fields { get; set; } = Array.Empty<SearchModelInFieldType>();

    public int Skip { get; set; }
    public int Take { get; set; } = 20;

    public class Validator : AbstractValidator<SearchModelRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Fields).ForEach(x => x.IsInEnum()).NotNull();
            RuleFor(x => x.Take).NotEmpty().LessThanOrEqualTo(100);
        }
    }
}