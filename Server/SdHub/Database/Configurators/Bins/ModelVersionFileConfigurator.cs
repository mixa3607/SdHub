using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Bins;

namespace SdHub.Database.Configurators.Bins;

public class ModelVersionFileConfigurator : IEntityTypeConfiguration<ModelVersionFileEntity>
{
    public void Configure(EntityTypeBuilder<ModelVersionFileEntity> builder)
    {
        builder.HasKey(x => new { x.FileId, x.ModelVersionId });
    }
}