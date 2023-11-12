using System;
using SdHub.Models.Enums;
using Reinforced.Typings.Fluent;
using SdHub.Models;
using System.IO;
using Rb.Itsd.TsGenerator;
using SdHub.Constants;
using SdHub.Models.Bins;
using SdHub.Models.Image;
using SdHub.Models.Upload;
using SdHub.Shared.AspErrorHandling;

namespace SdHub.TsConfigurators;

public class MiscTsConfigurator : ITsConfigurator
{
    public void Configure(ConfigurationBuilder builder)
    {
        var outFile = Path.Combine(ReinforcedTypingsConfiguration.ModelsRoot, "misc.models.ts");
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
        }, c => c.ExportTo(outFile));
        builder.ExportAsClasses(new Type[]
            {
                typeof(ModelStateErrors)
            }, c => c
                .WithPublicFields()
                .WithPublicProperties()
                .ExportTo(outFile))
            ;
    }
}