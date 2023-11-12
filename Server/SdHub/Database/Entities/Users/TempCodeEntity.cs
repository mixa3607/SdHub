using System;

namespace SdHub.Database.Entities.Users;

public class TempCodeEntity
{
    public long Id { get; set; }
    public string Key { get; set; } = null!;
    public string Code { get; set; } = null!;
    public int MaxAttempts { get; set; }
    public int UsedAttempts { get; set; }
    public DateTimeOffset ExpiredAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public bool Used { get; set; }
}