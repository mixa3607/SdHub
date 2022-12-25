using System;
using System.IO;
using Reinforced.Typings.Fluent;
using SdHub.Models.Bins.Vaes;
using SdHub.TsConfigurators.Extensions;
using SdHub.TsConfigurators.Shared;

namespace SdHub.TsConfigurators;

public class VaeTsConfigurator : ITsConfigurator
{
    public void Configure(ConfigurationBuilder builder)
    {
        var outFile = Path.Combine(ITsConfigurator.ModelsRoot, "vae.models.ts");
        builder.ExportAsInterfaces(new Type[]
            {
                typeof(VaeModel),
                typeof(VaeVersionModel),
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