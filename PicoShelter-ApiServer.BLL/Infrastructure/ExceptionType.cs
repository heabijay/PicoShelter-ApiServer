namespace PicoShelter_ApiServer.BLL.Infrastructure
{
    public enum ExceptionType
    {
        UNTYPED,
        MODEL_NOT_VALID,
        USERNAME_ALREADY_REGISTERED,
        EMAIL_ALREADY_REGISTERED,
        CREDENTIALS_INCORRECT,
        USER_NOT_FOUND,
        ALBUM_NOT_FOUND,
        YOU_NOT_OWNER_OF_IMAGE,
        ADMIN_KICK_DISALLOWED,
        USERCODE_ALREADY_TAKED,
        INTERNAL_FILE_ERROR,
        INPUT_IMAGE_INVALID
    }
}
