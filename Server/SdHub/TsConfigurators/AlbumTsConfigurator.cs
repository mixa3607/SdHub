using System;
using System.IO;
using Rb.Itsd.TsGenerator;
using Reinforced.Typings.Fluent;
using SdHub.Models.Album;

namespace SdHub.TsConfigurators;

public class AlbumTsConfigurator : ITsConfigurator
{
    public void Configure(ConfigurationBuilder builder)
    {
        var outFile = Path.Combine(ReinforcedTypingsConfiguration.ModelsRoot, "album.models.ts");
        builder.ExportAsInterfaces(new Type[]
            {
                typeof(AlbumModel),
                typeof(AlbumImageModel),
                typeof(GetAlbumRequest),
                typeof(GetAlbumResponse),
                typeof(AddAlbumImagesRequest),
                typeof(AddAlbumImagesResponse),
                typeof(CreateAlbumRequest),
                typeof(DeleteAlbumRequest),
                typeof(DeleteAlbumResponse),
                typeof(DeleteAlbumImagesRequest),
                typeof(DeleteAlbumImagesResponse),
                typeof(SearchAlbumRequest),
                typeof(EditAlbumRequest),
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