using PicoShelter_ApiServer.BLL.Infrastructure;
using System.Collections.Generic;

namespace PicoShelter_ApiServer.BLL.Extensions
{
    public static class ExceptionTypeMessage
    {
        private readonly static Dictionary<ExceptionType, string> ExceptionTypeMessages = new()
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
            },
            {
                ExceptionType.ALBUM_ACCESS_FORBIDDEN,
                "You haven't access to this album."
            },
            {
                ExceptionType.UNREGISTERED_QUALITY_FORBIDDEN,
                "Unlimited save not available for unregistered users."
            },
            {
                ExceptionType.UNREGISTERED_DELETEIN_FORBIDDEN,
                "Quality >75% not available for unregistered users."
            },
            {
                ExceptionType.UNREGISTERED_ISPUBLICPROP_FORBIDDEN,
                "Private upload not available for unregistered users."
            },
            {
                ExceptionType.UNREGISTERED_JOINTOALBUM_FORBIDDEN,
                "Join to albums not available for unregistered users."
            },
            {
                ExceptionType.CURRENT_EMAIL_WAS_ALREADY_CHANGED,
                "Current email was already changed since confirmation token was created"
            },
            {
                ExceptionType.CONFIRMATIONTYPE_UNSUPPORTED,
                "Selected confirmation type is unsupported for this method"
            },
            {
                ExceptionType.USER_ALREADY_INVITED,
                "Selected user already invited!"
            },
            {
                ExceptionType.USER_ALREADY_JOINED,
                "Selected user already joined!"
            },
            {
                ExceptionType.REPORT_ALREADY_PROCESSED,
                "Selected report was already processed!"
            },
            {
                ExceptionType.PROFILE_IS_BANNED,
                "Selected profile is banned!"
            },
            {
                ExceptionType.ADMIN_BAN_DISALLOWED,
                "You couldn't ban administrator."
            },
            {
                ExceptionType.USER_BANNED,
                "User banned."
            }
        };

        public static string GetMessage(this ExceptionType type)
        {
            return ExceptionTypeMessages[type];
        }
    }
}
