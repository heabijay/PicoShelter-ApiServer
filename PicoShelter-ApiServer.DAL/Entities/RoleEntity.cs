using PicoShelter_ApiServer.DAL.Abstract;
using System.Collections.Generic;

namespace PicoShelter_ApiServer.DAL.Entities
{
    public class RoleEntity : EntityBase
    {
        public string Name { get; set; }
        public virtual List<AccountEntity> Accounts { get; set; }
    }
}
