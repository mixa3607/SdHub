using FluentValidation;

namespace SdHub.Models.Image;

public class SearchImageRequestValidator : AbstractValidator<SearchImageRequest>
{
    public SearchImageRequestValidator()
    {
        RuleFor(x => x.Softwares).NotNull();
        RuleFor(x => x.Fields).ForEach(x => x.IsInEnum()).NotNull();
        RuleFor(x => x.OrderBy).IsInEnum();
        RuleFor(x => x.OrderByField).IsInEnum();
        RuleFor(x => x.Take).NotEmpty().LessThanOrEqualTo(100);
    }
}