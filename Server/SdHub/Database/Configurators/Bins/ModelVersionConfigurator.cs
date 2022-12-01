using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Bins;

namespace SdHub.Database.Configurators.Bins;

public class ModelVersionConfigurator : IEntityTypeConfiguration<ModelVersionEntity>
{
    public void Configure(EntityTypeBuilder<ModelVersionEntity> builder)
    {
        builder.Property(x => x.KnownNames).IsRequired();
    }
}