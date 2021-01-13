using PicoShelter_ApiServer.DAL.Abstract;
using System;

namespace PicoShelter_ApiServer.DAL.Entities
{
    public class AccountEntity : Entity
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public DateTime? LastPasswordChange { get; set; }

        public int RoleId { get; set; }
        public virtual RoleEntity Role { get; set; }

        public virtual ProfileEntity Profile { get; set; }
    }
}
