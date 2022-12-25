using System;
using System.IO;
using Reinforced.Typings.Fluent;
using SdHub.Models.Files;
using SdHub.TsConfigurators.Extensions;
using SdHub.TsConfigurators.Shared;

namespace SdHub.TsConfigurators;

public class FileTsConfigurator : ITsConfigurator
{
    public void Configure(ConfigurationBuilder builder)
    {
        var outFile = Path.Combine(ITsConfigurator.ModelsRoot, "file.models.ts");
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