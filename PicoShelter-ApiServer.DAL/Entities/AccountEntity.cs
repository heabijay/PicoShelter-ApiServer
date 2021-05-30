using PicoShelter_ApiServer.DAL.Abstract;
using System;
using System.Collections.Generic;

namespace PicoShelter_ApiServer.DAL.Entities
{
    public class AccountEntity : Entity
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public DateTime? LastCredentialsChange { get; set; }

        public int RoleId { get; set; }
        public virtual RoleEntity Role { get; set; }

        public virtual ProfileEntity Profile { get; set; }

        public virtual List<ConfirmationEntity> Confirmations { get; set; }
    }
}
