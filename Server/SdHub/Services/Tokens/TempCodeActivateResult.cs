namespace SdHub.Services.Tokens;

public enum TempCodeActivateResult
{
    Ok,
    NotFound,
    Lifetime,
    MaxAttemptsReached
}