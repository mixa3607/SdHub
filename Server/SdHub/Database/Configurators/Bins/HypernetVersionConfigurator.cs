using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Bins;

namespace SdHub.Database.Configurators.Bins;

public class HypernetVersionConfigurator : IEntityTypeConfiguration<HypernetVersionEntity>
{
    public void Configure(EntityTypeBuilder<HypernetVersionEntity> builder)
    {
        builder.Property(x => x.KnownNames).IsRequired();
    }
}