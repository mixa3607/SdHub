using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Files;

namespace SdHub.Database.Configurators.Files;

public class FileConfigurator : IEntityTypeConfiguration<FileEntity>
{
    public void Configure(EntityTypeBuilder<FileEntity> builder)
    {
        builder.Property(x => x.PathOnStorage).IsRequired();
        builder.Property(x => x.MimeType).IsRequired();
        builder.Property(x => x.Extension).IsRequired();
        builder.Property(x => x.Hash).IsRequired();
        builder.Navigation(e => e.Storage).AutoInclude();
    }
}