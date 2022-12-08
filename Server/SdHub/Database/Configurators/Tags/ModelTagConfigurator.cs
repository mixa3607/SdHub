using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Tags;

namespace SdHub.Database.Configurators.Tags;

public class ModelTagConfigurator : IEntityTypeConfiguration<ModelTagEntity>
{
    public void Configure(EntityTypeBuilder<ModelTagEntity> builder)
    {
        builder.HasKey(x => new { x.TagId, x.ModelId });
    }
}