using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Users;

namespace SdHub.Database.Configurators;

public class TempCodeConfigurator : IEntityTypeConfiguration<TempCodeEntity>
{
    public void Configure(EntityTypeBuilder<TempCodeEntity> builder)
    {
    }
}