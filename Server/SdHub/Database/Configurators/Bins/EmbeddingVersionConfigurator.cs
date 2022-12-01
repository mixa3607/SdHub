using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Bins;

namespace SdHub.Database.Configurators.Bins;

public class EmbeddingVersionConfigurator : IEntityTypeConfiguration<EmbeddingVersionEntity>
{
    public void Configure(EntityTypeBuilder<EmbeddingVersionEntity> builder)
    {
        builder.Property(x => x.KnownNames).IsRequired();
    }
}