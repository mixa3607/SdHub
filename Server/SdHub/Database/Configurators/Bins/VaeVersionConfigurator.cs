using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Bins;

namespace SdHub.Database.Configurators.Bins;

public class VaeVersionConfigurator : IEntityTypeConfiguration<VaeVersionEntity>
{
    public void Configure(EntityTypeBuilder<VaeVersionEntity> builder)
    {
        builder.Property(x => x.KnownNames).IsRequired();
    }
}