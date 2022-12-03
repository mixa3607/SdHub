using System;
using System.IO;
using Reinforced.Typings.Fluent;
using SdHub.Constants;
using SdHub.Models.Image;
using SdHub.TsConfigurators.Extensions;
using SdHub.TsConfigurators.Shared;

namespace SdHub.TsConfigurators;

public class ImageTsConfigurator : ITsConfigurator
{
    public void Configure(ConfigurationBuilder builder)
    {
        var outFile = Path.Combine(ITsConfigurator.ModelsRoot, "image.models.ts");
        builder.ExportAsInterfaces(new Type[]
            {
                typeof(DeleteImageRequest),
                typeof(DeleteImageResponse),
                typeof(EditImageRequest),
                typeof(EditImageResponse),
                typeof(GetImageRequest),
                typeof(GetImageResponse),
                typeof(CheckManageTokenRequest),
                typeof(CheckManageTokenResponse),
                typeof(SearchImageRequest),
                typeof(SearchImageResponse),
            }, c => c
                .SubsDatetimeOffsetToStr()
                .SubsTimespanToStr()
                .SubsGuidToStr()
                .WithPublicProperties()
                .ExportTo(outFile))
            ;
        builder.ExportAsEnums(new Type[]
        {
            typeof(SearchImageInFieldType),
            typeof(SearchImageOrderByFieldType),
            typeof(SearchImageOrderByType),
        }, c => c.ExportTo(outFile));
        builder.ExportAsClasses(new Type[]
            {
                typeof(SoftwareGeneratedTypes),
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