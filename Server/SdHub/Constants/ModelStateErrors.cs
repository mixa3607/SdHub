namespace SdHub.Constants;

public static class ModelStateErrors
{
    public const string UserDeleted = "USER_DELETED";
    public const string UserNotFound = "USER_NOT_FOUND";
    public const string UserEditNotAllowed = "USER_EDIT_NOT_ALLOWED";
    public const string UserWithEmailExist = "USER_WITH_EMAIL_EXIST";
    public const string UserWithLoginExist = "USER_WITH_LOGIN_EXIST";
    public const string UserRegistrationDisabled = "USER_REGISTRATION_DISABLED";
    public const string RefreshTokenNotExist = "REFRESH_TOKEN_NOT_EXIST";
    public const string EmailNotConfirmed = "EMAIL_NOT_CONFIRMED";
    public const string BadCreds = "BAD_CREDS";
    public const string BadConfirmationCode = "BAD_CONFIRMATION_CODE";
    public const string InvalidCaptcha = "INVALID_CAPTCHA";

    public const string ImageNotFound = "IMAGE_NOT_FOUND";
    public const string ImageIsPartOfGrid = "IMAGE_IS_PART_OF_GRID";

    public const string AlbumNotFound = "ALBUM_NOT_FOUND";
    public const string NotAlbumOwner = "NOT_ALBUM_OWNER";

    public const string GridNotFound = "GRID_NOT_FOUND";
    public const string NotGridOwner = "NOT_GRID_OWNER";
}