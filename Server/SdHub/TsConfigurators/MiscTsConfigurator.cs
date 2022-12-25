using System;
using SdHub.Models.Enums;
using SdHub.TsConfigurators.Shared;
using Reinforced.Typings.Fluent;
using SdHub.TsConfigurators.Extensions;
using SdHub.Models;
using SdHub.Services.ErrorHandling;
using System.IO;
using SdHub.Constants;
using SdHub.Models.Bins;
using SdHub.Models.Image;
using SdHub.Models.Upload;

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
                typeof(UploadedFileModel),
                typeof(UserSimpleModel),
                typeof(UserModel),
                typeof(ServerErrorResponse),
                typeof(PaginationResponse<>)
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
            typeof(SdVersion),
        }, c => c.ExportTo(outFile));
        builder.ExportAsClasses(new Type[]
            {
                typeof(ModelStateErrors)
            }, c => c
                .SubsDatetimeOffsetToStr()
                .SubsTimespanToStr()
                .SubsGuidToStr()
                .WithPublicFields()
                .WithPublicProperties()
                .ExportTo(outFile))
            ;
    }
}