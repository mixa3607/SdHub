using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SdHub.Database.Entities.User;

namespace SdHub.Database.Configurators.Users;

public class RefreshTokenConfigurator : IEntityTypeConfiguration<RefreshTokenEntity>
{
    public void Configure(EntityTypeBuilder<RefreshTokenEntity> builder)
    {
        builder.HasIndex(x => x.Token).IsUnique();
    }
}