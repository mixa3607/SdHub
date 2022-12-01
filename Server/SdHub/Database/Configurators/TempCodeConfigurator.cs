using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities;

namespace SdHub.Database.Configurators;

public class TempCodeConfigurator : IEntityTypeConfiguration<TempCodeEntity>
{
    public void Configure(EntityTypeBuilder<TempCodeEntity> builder)
    {
        builder.Property(x => x.Code).IsRequired();
        builder.Property(x => x.Identifier).IsRequired();
    }
}