using System;
using SdHub.Models.Enums;
using SdHub.TsConfigurators.Shared;
using Reinforced.Typings.Fluent;
using SdHub.Database.Entities.Files;
using SdHub.TsConfigurators.Extensions;
using SdHub.Models;
using SdHub.Services.ErrorHandling;
using System.IO;
using SdHub.Controllers;

namespace SdHub.TsConfigurators;

public class MiscTsConfigurator : ITsConfigurator
{
    public void Configure(ConfigurationBuilder builder)
    {
        var outFile = Path.Combine(ITsConfigurator.ModelsRoot, "misc.models.ts");
        builder.ExportAsInterfaces(new Type[]
            {
                typeof(FrontendSettings),
                typeof(EditImageModel),
                typeof(ImageParsedMetadataModel),
                typeof(ImageParsedMetadataTagModel),
                typeof(ImageModel),
                typeof(ImageOwnerModel),
                typeof(FileModel),
                typeof(UploadedFileModel),
                typeof(UserModel),
                typeof(ServerErrorResponse)
            }, c => c
                .SubsDatetimeOffsetToStr()
                .SubsTimespanToStr()
                .SubsGuidToStr()
                .WithPublicProperties()
                .ExportTo(outFile))
            ;
        builder.ExportAsEnums(new[]
        {
            typeof(AudienceType),
            typeof(CaptchaType),
        }, c => c.ExportTo(outFile));
    }
}