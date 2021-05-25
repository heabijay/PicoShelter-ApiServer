using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace PicoShelter_ApiServer
{
    public static class AuthOptions
    {
        public const string Issuer = "PicoShelterServer";
        public const string Audience = "PicoShelterClient";
        const string Key = "**MyVeryStrongPassword**";
        public static TimeSpan LifeTime = TimeSpan.FromDays(30);

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
        }
    }
}
