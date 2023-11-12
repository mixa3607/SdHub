using System;
using System.Collections.Generic;
using SdHub.Database.Entities.Images;

namespace SdHub.Database.Entities.Users;

public class UserEntity : IEntityWithDeletingFlag
{
    public long Id { get; set; }
    public Guid Guid { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }
    public string? DeleteReason { get; set; }

    public DateTimeOffset? EmailConfirmedAt { get; set; }
    public DateTimeOffset EmailConfirmationLastSend { get; set; }

    public long PlanId { get; set; }
    public UserPlanEntity? Plan { get; set; }

    public List<ImageEntity>? Images { get; set; }

    public string? About { get; set; }

    public string LoginNormalized { get; set; } = null!;
    public string Login { get; set; } = null!;

    public string EmailNormalized { get; set; } = null!;
    public string Email { get; set; } = null!;

    public string? PasswordHash { get; set; }

    public List<string> Roles { get; set; } = new List<string>();

    public List<RefreshTokenEntity>? RefreshTokens { get; set; }
    public List<UserApiTokenEntity>? ApiTokens { get; set; }
}