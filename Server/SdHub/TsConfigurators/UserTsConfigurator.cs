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
                typeof(ChangePasswordRequest),
                typeof(ChangePasswordResponse),
                typeof(ConfirmEmailRequest),
                typeof(ConfirmEmailResponse),
                typeof(LoginByPasswordRequest),
                typeof(LoginByRefreshTokenRequest),
                typeof(LoginResponse),
                typeof(RegisterRequest),
                typeof(RegisterResponse),
                typeof(ResetPasswordRequest),
                typeof(ResetPasswordResponse),
                typeof(GetMeRequest),
                typeof(GetMeResponse),
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