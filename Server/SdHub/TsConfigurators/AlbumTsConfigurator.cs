using System;
using System.IO;
using Reinforced.Typings.Fluent;
using SdHub.Models.Album;
using SdHub.TsConfigurators.Extensions;
using SdHub.TsConfigurators.Shared;

namespace SdHub.TsConfigurators;

public class AlbumTsConfigurator : ITsConfigurator
{
    public void Configure(ConfigurationBuilder builder)
    {
        var outFile = Path.Combine(ITsConfigurator.ModelsRoot, "album.models.ts");
        builder.ExportAsInterfaces(new Type[]
            {
                typeof(AlbumModel),
                typeof(AlbumImageModel),
                typeof(AddAlbumImagesRequest),
                typeof(AddAlbumImagesResponse),
                typeof(CreateAlbumRequest),
                typeof(DeleteAlbumRequest),
                typeof(DeleteAlbumImagesRequest),
                typeof(DeleteAlbumImagesResponse),
                typeof(SearchAlbumRequest),
                typeof(SearchAlbumResponse)
            }, c => c
                .SubsDatetimeOffsetToStr()
                .SubsTimespanToStr()
                .SubsGuidToStr()
                .WithPublicProperties()
                .ExportTo(outFile))
            ;
        builder.ExportAsEnums(new Type[]
        {
            typeof(SearchAlbumInFieldType),
            typeof(SearchAlbumOrderByType),
            typeof(SearchAlbumOrderByFieldType),
        }, c => c.ExportTo(outFile));
    }
}