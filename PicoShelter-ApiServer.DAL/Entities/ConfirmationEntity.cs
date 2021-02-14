using PicoShelter_ApiServer.DAL.Abstract;
using PicoShelter_ApiServer.DAL.Enums;
using System;

namespace PicoShelter_ApiServer.DAL.Entities
{
    public class ConfirmationEntity : Entity
    {
        public string Token { get; init; }
        public ConfirmationType Type { get; set; }
        public string Data { get; set; }
        public DateTime? ValidUntilUTC { get; set; }

        public int? AccountId { get; set; }
        public virtual AccountEntity Account { get; set; }
    }
}
