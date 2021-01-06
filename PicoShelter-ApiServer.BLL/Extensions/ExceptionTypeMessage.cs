using PicoShelter_ApiServer.BLL.Infrastructure;
using System.Collections.Generic;

namespace PicoShelter_ApiServer.Extensions
{
    public static class ExceptionTypeMessage
    {
        private static Dictionary<ExceptionType, string> ExceptionTypeMessages = new()
        {
            {
                ExceptionType.MODEL_NOT_VALID,
                "Here is some validation errors in request." 
            },
            {
                ExceptionType.USERNAME_ALREADY_REGISTERED,
                "Selected username already registered."
            },
            {
                ExceptionType.EMAIL_ALREADY_REGISTERED,
                "Selected email already registered."
            },
            {
                ExceptionType.CREDENTIALS_INCORRECT,
                "Entered username or password is incorrect."
            },
            {
                ExceptionType.USER_NOT_FOUND,
                "Selected user not found."
            },
            {
                ExceptionType.ALBUM_NOT_FOUND,
                "Selected album not found."
            },
            {
                ExceptionType.YOU_NOT_OWNER_OF_IMAGE,
                "You must be owner of this image to do that."
            },
            {
                ExceptionType.ADMIN_KICK_DISALLOWED,
                "You couldn't kick administrator." 
            },
            {
                ExceptionType.USERCODE_ALREADY_TAKED,
                "Selected usercode already taked."
            }
            ,
            {
                ExceptionType.INTERNAL_FILE_ERROR,
                "You're file record was found, but something was wrong with binary data in file. You could try again later."
            },
            {
                ExceptionType.INPUT_IMAGE_INVALID,
                "Uploaded image is incorrect or unsupported."
            }
        };

        public static string GetMessage(this ExceptionType type)
        {
            return ExceptionTypeMessages[type];
        }
    }
}
