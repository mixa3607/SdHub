using System;
using System.IO;
using Rb.Itsd.TsGenerator;
using Reinforced.Typings.Fluent;
using SdHub.Constants;
using SdHub.Models.User;

namespace SdHub.TsConfigurators;

public class UserTsConfigurator : ITsConfigurator
{
    public void Configure(ConfigurationBuilder builder)
    {
        var outFile = Path.Combine(ReinforcedTypingsConfiguration.ModelsRoot, "user.models.ts");
        builder.ExportAsInterfaces(new Type[]
            {
                typeof(SendResetPasswordEmailRequest),
                typeof(SendResetPasswordEmailResponse),
                typeof(ResetPasswordRequest),
                typeof(ResetPasswordResponse),
                typeof(ConfirmEmailRequest),
                typeof(ConfirmEmailResponse),
                typeof(LoginByPasswordRequest),
                typeof(LoginByRefreshTokenRequest),
                typeof(LoginResponse),
                typeof(RegisterRequest),
                typeof(RegisterResponse),
                typeof(GetMeRequest),
                typeof(GetMeResponse),
                typeof(SendEmailConfirmationEmailRequest),
                typeof(SendEmailConfirmationEmailResponse),
                typeof(EditUserRequest),
                typeof(EditUserResponse),
                typeof(GetUserRequest),
                typeof(GetUserResponse),
            }, c => c
                .SubsDatetimeOffsetToStr()
                .SubsTimespanToStr()
                .SubsGuidToStr()
                .WithPublicProperties()
                .ExportTo(outFile))
            ;

        builder.ExportAsClasses(new Type[]
            {
                typeof(UserRoleTypes),
            }, c => c
                .WithPublicFields()
                .WithPublicProperties()
                .ExportTo(outFile))
            ;
    }
}