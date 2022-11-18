using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.User;

namespace SdHub.Database.Configurators.Users;

public class UserConfigurator : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasIndex(x => new { x.Guid }).IsUnique();
        builder.HasIndex(x => new { x.LoginNormalized, x.DeletedAt }).IsUnique();
        builder.Property(x => x.LoginNormalized).IsRequired();
        builder.Property(x => x.PasswordHash).IsRequired();
        builder.Property(x => x.Roles).IsRequired();
    }
}