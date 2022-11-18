using System;
using System.Collections.Generic;
using SdHub.Database.Entities.Images;
using SdHub.Database.Entities.Users;

namespace SdHub.Database.Entities.User;

public class UserEntity
{
    public long Id { get; set; }
    public Guid Guid { get; set; } = Guid.NewGuid();
    public DateTimeOffset? DeletedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    public long PlanId { get; set; }
    public UserPlanEntity? Plan { get; set; }
    public List<ImageEntity>? Images { get; set; }
    public bool IsAnonymous { get; set; }

    //for registered
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
    public string? LoginNormalized { get; set; }
    public string? PasswordHash { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
    public List<RefreshTokenEntity>? RefreshTokens { get; set; }
}