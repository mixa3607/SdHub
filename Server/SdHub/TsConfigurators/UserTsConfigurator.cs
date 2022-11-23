using System;
using System.IO;
using Reinforced.Typings.Fluent;
using SdHub.Models.User;
using SdHub.TsConfigurators.Extensions;
using SdHub.TsConfigurators.Shared;

namespace SdHub.TsConfigurators;

public class UserTsConfigurator : ITsConfigurator
{
    public void Configure(ConfigurationBuilder builder)
    {
        var outFile = Path.Combine(ITsConfigurator.ModelsRoot, "user.models.ts");
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
            }, c => c
                .SubsDatetimeOffsetToStr()
                .SubsTimespanToStr()
                .SubsGuidToStr()
                .WithPublicProperties()
                .ExportTo(outFile))
            ;
        builder.ExportAsEnums(new Type[]
        {
        }, c => c.ExportTo(outFile));
    }
}