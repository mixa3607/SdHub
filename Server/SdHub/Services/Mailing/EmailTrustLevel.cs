namespace SdHub.Services.Mailing;

public enum EmailTrustLevel : byte
{
    Deny = 1,
    Suspicious = 10,
    Allow = 20,
}