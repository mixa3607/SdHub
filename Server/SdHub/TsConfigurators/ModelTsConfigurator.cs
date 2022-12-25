using System;
using System.IO;
using Reinforced.Typings.Fluent;
using SdHub.Models.Album;
using SdHub.Models.Bins;
using SdHub.TsConfigurators.Extensions;
using SdHub.TsConfigurators.Shared;

namespace SdHub.TsConfigurators;

public class ModelTsConfigurator : ITsConfigurator
{
    public void Configure(ConfigurationBuilder builder)
    {
        var outFile = Path.Combine(ITsConfigurator.ModelsRoot, "model.models.ts");
        builder.ExportAsInterfaces(new Type[]
            {
                typeof(ModelModel),
                typeof(ModelVersionModel),
                typeof(ModelVersionFileModel),

                typeof(SearchModelRequest),
                typeof(GetModelRequest),
                typeof(CreateModelRequest),
                typeof(EditModelRequest),
                typeof(DeleteModelRequest),
                typeof(DeleteModelResponse),

                typeof(AddModelVersionRequest),
                typeof(EditModelVersionRequest),
                typeof(DeleteModelVersionRequest),
            }, c => c
                .SubsDatetimeOffsetToStr()
                .SubsTimespanToStr()
                .SubsGuidToStr()
                .WithPublicProperties()
                .ExportTo(outFile))
            ;
        builder.ExportAsEnums(new Type[]
        {
            typeof(SearchModelInFieldType),
        }, c => c.ExportTo(outFile));
    }
}