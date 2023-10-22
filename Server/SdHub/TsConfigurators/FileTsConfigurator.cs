using System;
using System.IO;
using Rb.Itsd.TsGenerator;
using Reinforced.Typings.Fluent;
using SdHub.Models.Files;

namespace SdHub.TsConfigurators;

public class FileTsConfigurator : ITsConfigurator
{
    public void Configure(ConfigurationBuilder builder)
    {
        var outFile = Path.Combine(ReinforcedTypingsConfiguration.ModelsRoot, "file.models.ts");
        builder.ExportAsInterfaces(new Type[]
            {
                typeof(FileModel),
                typeof(DirectoryModel),
                typeof(ImportFileRequest),
                typeof(SearchFileRequest),
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