using System;
using System.IO;
using Rb.Itsd.TsGenerator;
using Reinforced.Typings.Fluent;
using SdHub.Constants;
using SdHub.Models.Image;

namespace SdHub.TsConfigurators;

public class ImageTsConfigurator : ITsConfigurator
{
    public void Configure(ConfigurationBuilder builder)
    {
        var outFile = Path.Combine(ReinforcedTypingsConfiguration.ModelsRoot, "image.models.ts");
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
                .WithPublicFields()
                .WithPublicProperties()
                .ExportTo(outFile))
            ;
    }
}