using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Files;

namespace SdHub.Database.Configurators.Files;

public class FileConfigurator : IEntityTypeConfiguration<FileEntity>
{
    public void Configure(EntityTypeBuilder<FileEntity> builder)
    {
        builder.HasIndex(x => new { x.PathOnStorage, x.StorageName }).IsUnique();
        builder.Property(x => x.PathOnStorage).IsRequired();
        builder.Property(x => x.MimeType).IsRequired();
        builder.Property(x => x.Extension).IsRequired();
        builder.Property(x => x.Hash).IsRequired();
    }
}