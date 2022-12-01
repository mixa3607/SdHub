using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Grids;

namespace SdHub.Database.Configurators.Grids;

public class GridConfigurator : IEntityTypeConfiguration<GridEntity>
{
    public void Configure(EntityTypeBuilder<GridEntity> builder)
    {
        builder.Property(x => x.ShortToken).IsRequired();
    }
}