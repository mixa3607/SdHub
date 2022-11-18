using System;

namespace SdHub.Models.User;

public class ResetPasswordResponse
{
    public string? SendToEmail { get; set; }
    public DateTimeOffset ExpiredAt { get; set; }
}