using FluentValidation;

namespace SdHub.Models.Upload;

public class UploadRequestValidator : AbstractValidator<UploadRequest>
{
    public UploadRequestValidator()
    {
        RuleFor(x => x.Files).NotEmpty();
    }
}
