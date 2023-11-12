using FluentValidation;

namespace SdHub.Models.Samples;

public class CreateGenerationSampleRequest
{
    public long? ModelId { get; set; }
    public long? HypernetId { get; set; }
    public long? VaeId { get; set; }
    public long? EmbeddingId { get; set; }
    public string? ImageShortToken { get; set; }

    public class Validator : AbstractValidator<CreateGenerationSampleRequest>
    {
        public Validator()
        {
            RuleFor(x => x).Must(x =>
                !(x.EmbeddingId == default &&
                  x.VaeId == default &&
                  x.ModelId == default &&
                  x.HypernetId == default));
            RuleFor(x => x.ImageShortToken).NotEmpty();
        }
    }
}