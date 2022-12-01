using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Grids;

namespace SdHub.Database.Configurators.Grids;

public class GridImageConfigurator : IEntityTypeConfiguration<GridImageEntity>
{
    public void Configure(EntityTypeBuilder<GridImageEntity> builder)
    {
        builder.HasKey(x => new { x.GridId, x.ImageId });
    }
}