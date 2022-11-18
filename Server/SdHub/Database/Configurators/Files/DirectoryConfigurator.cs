using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Files;

namespace SdHub.Database.Configurators.Files;

public class DirectoryConfigurator : IEntityTypeConfiguration<DirectoryEntity>
{
    public void Configure(EntityTypeBuilder<DirectoryEntity> builder)
    {
        builder.HasIndex(x => new { x.PathOnStorage, x.StorageName }).IsUnique();
        builder.Property(x => x.PathOnStorage).IsRequired();
    }
}