using System;
using System.Collections.Generic;
using SdHub.Database.Entities.Images;

namespace SdHub.Database.Entities.Users;

public class UserEntity
{
    public long Id { get; set; }
    public Guid Guid { get; set; } = Guid.NewGuid();

    public string? DeleteReason { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public DateTimeOffset? EmailConfirmedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset EmailConfirmationLastSend { get; set; }
    public long PlanId { get; set; }
    public UserPlanEntity? Plan { get; set; }
    public List<ImageEntity>? Images { get; set; }
    public bool IsAnonymous { get; set; }
    public string? About { get; set; }

    //for registered
    public string? LoginNormalized { get; set; }
    private string? _login;
    public string? Login
    {
        get => _login;
        set
        {
            _login = value;
            LoginNormalized = _login?.Normalize().ToUpper();
        }
    }
    public string? EmailNormalized { get; set; }
    private string? _email;
    public string? Email
    {
        get => _email;
        set
        {
            _email = value;
            EmailNormalized = _email?.Normalize().ToUpper();
        }
    }
    public string? PasswordHash { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
    public List<RefreshTokenEntity>? RefreshTokens { get; set; }
    public List<UserApiTokenEntity>? ApiTokens { get; set; }
}