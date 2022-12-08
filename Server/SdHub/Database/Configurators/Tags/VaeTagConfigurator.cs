using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Tags;

namespace SdHub.Database.Configurators.Tags;

public class VaeTagConfigurator : IEntityTypeConfiguration<VaeTagEntity>
{
    public void Configure(EntityTypeBuilder<VaeTagEntity> builder)
    {
        builder.HasKey(x => new { x.TagId, x.VaeId });
    }
}