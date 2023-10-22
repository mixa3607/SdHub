using System;
using System.IO;
using Rb.Itsd.TsGenerator;
using Reinforced.Typings.Fluent;
using SdHub.Models.Bins.Embeddings;

namespace SdHub.TsConfigurators;

public class EmbeddingTsConfigurator : ITsConfigurator
{
    public void Configure(ConfigurationBuilder builder)
    {
        var outFile = Path.Combine(ReinforcedTypingsConfiguration.ModelsRoot, "embedding.models.ts");
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