using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Bins;

namespace SdHub.Database.Configurators.Bins;

public class ModelFileConfigurator : IEntityTypeConfiguration<ModelFileEntity>
{
    public void Configure(EntityTypeBuilder<ModelFileEntity> builder)
    {
        builder.HasKey(x => new { x.FileId, x.ModelId });
    }
}