using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Tags;

namespace SdHub.Database.Configurators.Tags;

public class HypernetTagConfigurator : IEntityTypeConfiguration<HypernetTagEntity>
{
    public void Configure(EntityTypeBuilder<HypernetTagEntity> builder)
    {
        builder.HasKey(x => new { x.TagId, x.HypernetId });
    }
}