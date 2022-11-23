namespace SdHub.Constants;

public static class ModelStateErrors
{
    public const string UserNotFound = "USER_NOT_FOUND";
    public const string EmailNotConfirmed = "EMAIL_NOT_CONFIRMED";
    public const string BadCreds = "BAD_CREDS";
    public const string BadConfirmationCode = "BAD_CONFIRMATION_CODE";
    public const string UserWithEmailExist = "USER_WITH_EMAIL_EXIST";
    public const string UserWithLoginExist = "USER_WITH_LOGIN_EXIST";
    public const string InvalidCaptcha = "INVALID_CAPTCHA";
    public const string RefreshTokenNotExist = "REFRESH_TOKEN_NOT_EXIST";
}