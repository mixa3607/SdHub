using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Images;

namespace SdHub.Database.Configurators.Images;

public class ImageParsedMetadataTagConfigurator : IEntityTypeConfiguration<ImageParsedMetadataTagEntity>
{
    public void Configure(EntityTypeBuilder<ImageParsedMetadataTagEntity> builder)
    {
        builder.Property(x => x.Software).IsRequired();
        builder.Property(x => x.Source).IsRequired();
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.Value).IsRequired();
    }
}