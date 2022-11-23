using System;
using SdHub.Services.Tokens;

namespace SdHub.Database.Entities;

public class TempCodeEntity
{
    public long Id { get; set; }
    public TempCodeType CodeType { get; set; }
    public string? Identifier { get; set; }
    public string? Code { get; set; }
    public int MaxAttempts { get; set; }
    public int CurrAttempts { get; set; }
    public DateTimeOffset ExpiredAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public bool Used { get; set; }
}