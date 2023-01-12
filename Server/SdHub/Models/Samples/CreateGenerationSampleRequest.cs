using FluentValidation;

namespace SdHub.Models.Samples;

public class CreateGenerationSampleRequest
{
    public long? ModelVersionId { get; set; }
    public long? HypernetVersionId { get; set; }
    public long? VaeVersionId { get; set; }
    public long? EmbeddingVersionId { get; set; }
    public string? ImageShortToken { get; set; }

    public class Validator : AbstractValidator<CreateGenerationSampleRequest>
    {
        public Validator()
        {
            RuleFor(x => x).Must(x =>
                !(x.EmbeddingVersionId == default &&
                  x.VaeVersionId == default &&
                  x.ModelVersionId == default &&
                  x.HypernetVersionId == default));
            RuleFor(x => x.ImageShortToken).NotEmpty();
        }
    }
}