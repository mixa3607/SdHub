using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Albums;

namespace SdHub.Database.Configurators.Albums;

public class AlbumImageConfigurator : IEntityTypeConfiguration<AlbumImageEntity>
{
    public void Configure(EntityTypeBuilder<AlbumImageEntity> builder)
    {
        builder.HasKey(x => new { x.AlbumId, x.ImageId, x.GridId });
    }
}