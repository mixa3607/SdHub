using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Bins;

namespace SdHub.Database.Configurators.Bins;

public class GenerationSampleConfigurator : IEntityTypeConfiguration<GenerationSampleEntity>
{
    public void Configure(EntityTypeBuilder<GenerationSampleEntity> builder)
    {
    }
}