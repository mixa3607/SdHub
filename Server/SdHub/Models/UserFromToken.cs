using System;
using System.Collections.Generic;

namespace SdHub.Models;

public class UserFromToken
{
    public Guid Guid { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Login { get; set; }
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
    public string? JwtToken { get; set; }

    public UserModel? User { get; set; }
}