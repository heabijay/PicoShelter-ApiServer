using PicoShelter_ApiServer.DAL.Abstract;
using System;

namespace PicoShelter_ApiServer.DAL.Entities
{
    public class BanEntity : EntityBase
    {
        public DateTime UntilDate { get; set; }
        public string Comment { get; set; }

        public int UserId { get; set; }
        public virtual AccountEntity User { get; set; }

        public int AdminId { get; set; }
        public virtual ProfileEntity Admin { get; set; }  
    }
}
