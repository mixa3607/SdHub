using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Images;

namespace SdHub.Database.Configurators.Images;

public class ImageParsedMetadataConfigurator : IEntityTypeConfiguration<ImageParsedMetadataEntity>
{
    public void Configure(EntityTypeBuilder<ImageParsedMetadataEntity> builder)
    {
        builder.HasKey(x => x.ImageId);
        builder
            .HasOne(x => x.Image)
            .WithOne(x => x.ParsedMetadata)
            .HasForeignKey<ImageParsedMetadataEntity>(x => x.ImageId);
    }
}