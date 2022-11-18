using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Images;

namespace SdHub.Database.Configurators.Images;

public class ImageUploaderConfigurator : IEntityTypeConfiguration<ImageUploaderEntity>
{
    public void Configure(EntityTypeBuilder<ImageUploaderEntity> builder)
    {
        builder.HasIndex(x => x.IpAddress).IsUnique();
        builder.Property(x => x.IpAddress).IsRequired();
    }
}