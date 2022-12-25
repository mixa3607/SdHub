using System;
using System.IO;
using Reinforced.Typings.Fluent;
using SdHub.Models.Bins.Embeddings;
using SdHub.TsConfigurators.Extensions;
using SdHub.TsConfigurators.Shared;

namespace SdHub.TsConfigurators;

public class EmbeddingTsConfigurator : ITsConfigurator
{
    public void Configure(ConfigurationBuilder builder)
    {
        var outFile = Path.Combine(ITsConfigurator.ModelsRoot, "embedding.models.ts");
        builder.ExportAsInterfaces(new Type[]
            {
                typeof(EmbeddingModel),
                typeof(EmbeddingVersionModel),
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