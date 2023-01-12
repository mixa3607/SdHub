using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Bins;

namespace SdHub.Database.Configurators.Bins;

public class ModelVersionConfigurator : IEntityTypeConfiguration<ModelVersionEntity>
{
    public void Configure(EntityTypeBuilder<ModelVersionEntity> builder)
    {
        builder.HasIndex(x => new { x.Id, x.Order });
        builder.Property(x => x.KnownNames).IsRequired();
        builder.HasMany(x => x.Samples)
            .WithOne(x => x.ModelVersion)
            .HasForeignKey(x => x.ModelVersionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}