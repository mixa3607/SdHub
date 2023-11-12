namespace SdHub.Services.TempCodes;

public enum TempCodeActivateResult
{
    Ok,
    Used,
    NotFound,
    Lifetime,
    MaxAttemptsReached
}