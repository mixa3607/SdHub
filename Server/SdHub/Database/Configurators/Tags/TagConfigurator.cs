using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Tags;

namespace SdHub.Database.Configurators.Tags;

public class TagConfigurator : IEntityTypeConfiguration<TagEntity>
{
    public void Configure(EntityTypeBuilder<TagEntity> builder)
    {
        builder.HasIndex(x => x.Value).IsUnique();
        builder.Property(x => x.Value).IsRequired();
    }
}