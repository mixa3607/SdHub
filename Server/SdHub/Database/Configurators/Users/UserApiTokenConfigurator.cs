using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Users;

namespace SdHub.Database.Configurators.Users;

public class UserApiTokenConfigurator : IEntityTypeConfiguration<UserApiTokenEntity>
{
    public void Configure(EntityTypeBuilder<UserApiTokenEntity> builder)
    {
        builder.HasKey(x => x.Token);
    }
}