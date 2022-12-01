using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SdHub.Database.Entities.Bins;

namespace SdHub.Database.Configurators.Bins;

public class EmbeddingConfigurator : IEntityTypeConfiguration<EmbeddingEntity>
{
    public void Configure(EntityTypeBuilder<EmbeddingEntity> builder)
    {
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.Trigger).IsRequired();
    }
}