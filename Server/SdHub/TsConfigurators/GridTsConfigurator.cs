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
                typeof(GetGridRequest),
                typeof(GetGridResponse),
                typeof(SearchGridRequest),
                typeof(EditGridRequest),
                typeof(EditGridResponse),
                typeof(DeleteGridRequest),
                typeof(DeleteGridResponse),
                typeof(UploadGridCheckInputRequest),
            }, c => c
                .SubsDatetimeOffsetToStr()
                .SubsTimespanToStr()
                .SubsGuidToStr()
                .WithPublicProperties()
                .ExportTo(outFile))
            ;
        builder.ExportAsEnums(new Type[]
        {
            typeof(SearchGridInFieldType),
            typeof(SearchGridOrderByFieldType),
            typeof(SearchGridOrderByType),
        }, c => c.ExportTo(outFile));
    }
}