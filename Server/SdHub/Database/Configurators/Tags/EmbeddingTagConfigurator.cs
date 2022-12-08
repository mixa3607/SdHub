using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Tags;

namespace SdHub.Database.Configurators.Tags;

public class EmbeddingTagConfigurator : IEntityTypeConfiguration<EmbeddingTagEntity>
{
    public void Configure(EntityTypeBuilder<EmbeddingTagEntity> builder)
    {
        builder.HasKey(x => new { x.TagId, x.EmbeddingId });
    }
}