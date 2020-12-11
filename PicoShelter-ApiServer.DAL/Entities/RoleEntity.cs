using PicoShelter_ApiServer.DAL.Abstract;
using System.Collections.Generic;

namespace PicoShelter_ApiServer.DAL.Entities
{
    public class RoleEntity : Entity
    {
        public string Name { get; set; }
        public virtual List<AccountEntity> Accounts { get; set; }
    }
}
