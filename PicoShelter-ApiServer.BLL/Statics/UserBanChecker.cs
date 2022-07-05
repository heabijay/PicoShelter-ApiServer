using PicoShelter_ApiServer.BLL.Infrastructure;
using PicoShelter_ApiServer.DAL.Entities;
using PicoShelter_ApiServer.DAL.Interfaces;
using System;

namespace PicoShelter_ApiServer.BLL.Statics
{
    public static class UserBanChecker
    {
        public static bool IsUserBanned(IUnitOfWork db, int accountId, out BanEntity ban)
        {
            ban = db.Bans.FirstOrDefault(t => t.UserId == accountId && t.UntilDate > DateTime.UtcNow);

            return ban is not null;
        }

        public static bool IsUserBanned(IUnitOfWork db, int accountId)
            => IsUserBanned(db, accountId, out _);


        /// <exception cref="HandlingException"></exception>
        public static void ThrowIfUserBanned(IUnitOfWork db, int accountId)
        {
            if (IsUserBanned(db, accountId, out BanEntity ban))
                throw new HandlingException(ExceptionType.USER_BANNED, new
                {
                    Comment = ban.Comment,
                    FromDate = ban.CreatedDateUTC,
                    ToDate = ban.UntilDate
                });
        }
    }
}
