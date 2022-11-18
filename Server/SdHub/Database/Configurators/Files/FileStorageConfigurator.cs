using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Files;

namespace SdHub.Database.Configurators.Files;

public class FileStorageConfigurator : IEntityTypeConfiguration<FileStorageEntity>
{
    public void Configure(EntityTypeBuilder<FileStorageEntity> builder)
    {
        builder.HasKey(x => x.Name);
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.BaseUrl).IsRequired();
    }
}