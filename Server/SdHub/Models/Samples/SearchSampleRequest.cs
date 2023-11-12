using FluentValidation;

namespace SdHub.Models.Samples;

public class SearchSampleRequest
{
    public long ModelId { get; set; }
    public long HypernetId { get; set; }
    public long VaeId { get; set; }
    public long EmbeddingId { get; set; }

    public class Validator : AbstractValidator<SearchSampleRequest>
    {
        public Validator()
        {
            RuleFor(x => x).Must(x =>
                !(x.EmbeddingId == default &&
                  x.VaeId == default &&
                  x.ModelId == default &&
                  x.HypernetId == default)
            ).WithMessage("Any filed must be filled");
        }
    }
}