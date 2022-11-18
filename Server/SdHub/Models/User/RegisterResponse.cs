using System;

namespace SdHub.Models.User;

public class RegisterResponse
{
    public string? SendToEmail { get; set; }
    public DateTimeOffset ExpiredAt { get; set; }
}