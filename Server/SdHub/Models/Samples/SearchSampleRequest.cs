using FluentValidation;

namespace SdHub.Models.Samples;

public class SearchSampleRequest
{
    public long ModelId { get; set; }
    public long HypernetId { get; set; }
    public long VaeId { get; set; }
    public long EmbeddingId { get; set; }
    public long ModelVersionId { get; set; }
    public long HypernetVersionId { get; set; }
    public long VaeVersionId { get; set; }
    public long EmbeddingVersionId { get; set; }

    public class Validator : AbstractValidator<SearchSampleRequest>
    {
        public Validator()
        {
            RuleFor(x => x).Must(x =>
                !(x.EmbeddingId == default && x.EmbeddingVersionId == default &&
                  x.VaeId == default && x.VaeVersionId == default &&
                  x.ModelId == default && x.ModelVersionId == default &&
                  x.HypernetId == default && x.HypernetVersionId == default)
            ).WithMessage("Any filed must be filled");
        }
    }
}