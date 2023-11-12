using System.Collections.Generic;

namespace SdHub.Services.User;

public class UserUpdate
{
    public string? Login { get; set; }
    public string? Email { get; set; }
    public long? PlanId { get; set; }
    public string? About { get; set; }
    public string? PasswordHash { get; set; }
    public IReadOnlyList<string>? Roles { get; set; }
}