using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Files;

namespace SdHub.Database.Configurators.Files;

public class DirectoryConfigurator : IEntityTypeConfiguration<DirectoryEntity>
{
    public void Configure(EntityTypeBuilder<DirectoryEntity> builder)
    {
        builder.Property(x => x.PathOnStorage).IsRequired();
        builder.Navigation(e => e.Storage).AutoInclude();
    }
}