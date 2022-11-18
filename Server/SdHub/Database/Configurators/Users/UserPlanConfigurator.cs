using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SdHub.Database.Entities.Users;

namespace SdHub.Database.Configurators.Users;

public class UserPlanConfigurator : IEntityTypeConfiguration<UserPlanEntity>
{
    public void Configure(EntityTypeBuilder<UserPlanEntity> builder)
    {
        builder.Property(x => x.Name).IsRequired();
    }
}