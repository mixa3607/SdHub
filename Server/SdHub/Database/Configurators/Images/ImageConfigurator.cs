using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Images;

namespace SdHub.Database.Configurators.Images;

public class ImageConfigurator : IEntityTypeConfiguration<ImageEntity>
{
    public void Configure(EntityTypeBuilder<ImageEntity> builder)
    {
        builder.HasIndex(x => x.ShortToken).IsUnique();
        builder.Property(x => x.ShortToken).IsRequired();
        builder.Property(x => x.ManageToken).IsRequired();
    }
}