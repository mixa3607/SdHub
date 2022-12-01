using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Bins;

namespace SdHub.Database.Configurators.Bins;

public class HypernetConfigurator : IEntityTypeConfiguration<HypernetEntity>
{
    public void Configure(EntityTypeBuilder<HypernetEntity> builder)
    {
        builder.Property(x => x.Name).IsRequired();
    }
}