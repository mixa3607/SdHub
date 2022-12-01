using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Bins;

namespace SdHub.Database.Configurators.Bins;

public class VaeConfigurator : IEntityTypeConfiguration<VaeEntity>
{
    public void Configure(EntityTypeBuilder<VaeEntity> builder)
    {
        builder.Property(x => x.Name).IsRequired();
    }
}