using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Albums;

namespace SdHub.Database.Configurators.Albums;

public class AlbumImageConfigurator : IEntityTypeConfiguration<AlbumImageEntity>
{
    public void Configure(EntityTypeBuilder<AlbumImageEntity> builder)
    {
        builder.HasIndex(x => new { x.AlbumId, x.ImageId }).IsUnique();
        builder.HasIndex(x => new { x.AlbumId, x.GridId }).IsUnique();
        builder.Property(x => x.GridId).IsRequired(false);
        builder.Property(x => x.ImageId).IsRequired(false);
    }
}