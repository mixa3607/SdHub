using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Bins;

namespace SdHub.Database.Configurators.Bins;

public class ModelConfigurator : IEntityTypeConfiguration<ModelEntity>
{
    public void Configure(EntityTypeBuilder<ModelEntity> builder)
    {
        builder.Property(x => x.Name).IsRequired();
        builder.HasMany(x => x.Files)
            .WithOne(x => x.Model)
            .HasForeignKey(x => x.ModelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}