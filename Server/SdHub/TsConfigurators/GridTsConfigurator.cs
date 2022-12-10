using System;
using System.IO;
using Reinforced.Typings.Fluent;
using SdHub.Models.Grid;
using SdHub.Models.Upload;
using SdHub.TsConfigurators.Extensions;
using SdHub.TsConfigurators.Shared;

namespace SdHub.TsConfigurators;

public class GridTsConfigurator : ITsConfigurator
{
    public void Configure(ConfigurationBuilder builder)
    {
        var outFile = Path.Combine(ITsConfigurator.ModelsRoot, "grid.models.ts");
        builder.ExportAsInterfaces(new Type[]
            {
                typeof(GridModel),
                typeof(GridImageModel),
                typeof(UploadGridRequest),
                typeof(UploadGridResponse),
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