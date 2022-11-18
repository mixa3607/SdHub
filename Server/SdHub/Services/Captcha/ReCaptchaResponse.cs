using System;
using Newtonsoft.Json;

namespace SdHub.Services.Captcha;

public class ReCaptchaResponse
{
    public bool Success { get; set; }
    public double Score { get; set; }
    public string? Action { get; set; }
    [JsonProperty("challenge_ts")]
    public DateTime ChallengeTimestamp { get; set; }
    public string? Hostname { get; set; }
    public string[]? ErrorCodes { get; set; }
}