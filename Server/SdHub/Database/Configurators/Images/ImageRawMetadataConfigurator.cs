using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Images;

namespace SdHub.Database.Configurators.Images;

public class ImageRawMetadataConfigurator : IEntityTypeConfiguration<ImageRawMetadataEntity>
{
    public void Configure(EntityTypeBuilder<ImageRawMetadataEntity> builder)
    {
        builder.HasKey(x => x.ImageId);
        builder
            .HasOne(x => x.Image)
            .WithOne(x => x.RawMetadata)
            .HasForeignKey<ImageRawMetadataEntity>(x => x.ImageId);
        builder.Property(x => x.Directories).HasColumnType("jsonb").IsRequired();
    }
}