using FluentValidation;

namespace SdHub.Models.User;

public class ResetPasswordRequest
{
    public string? Login { get; set; }
    public string? Code { get; set; }
    public string? NewPassword { get; set; }
}

public class GetUserRequest
{
    public string? Login { get; set; }
}

public class GetUserResponse
{
    public UserModel? User { get; set; }
}

public class GetUserRequestValidator : AbstractValidator<GetUserRequest>
{
    public GetUserRequestValidator()
    {
        RuleFor(x => x.Login).NotEmpty();
    }
}


public class EditUserRequest
{
    public string? Login { get; set; }
    public string? About { get; set; }
}

public class EditUserResponse
{
    public UserModel? User { get; set; }
}

public class EditUserRequestValidator : AbstractValidator<EditUserRequest>
{
    public EditUserRequestValidator()
    {
        RuleFor(x => x.Login).NotEmpty();
        RuleFor(x => x.About).MaximumLength(4000);
    }
}