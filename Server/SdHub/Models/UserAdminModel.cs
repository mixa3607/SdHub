using System;
using System.Collections.Generic;
using SdHub.Database.Entities.Users;

namespace SdHub.Models;

public class UserAdminModel
{
    public Guid Guid { get; set; }

    public string? Login { get; set; }
    public string? LoginNormalized { get; set; }
    public string? Email { get; set; }
    public string? EmailNormalized { get; set; }
    public bool IsAnonymous { get; set; }

    public string? DeleteReason { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public DateTimeOffset? EmailConfirmedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset EmailConfirmationLastSend { get; set; }
    public UserPlanEntity? Plan { get; set; }
    public string? About { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
}