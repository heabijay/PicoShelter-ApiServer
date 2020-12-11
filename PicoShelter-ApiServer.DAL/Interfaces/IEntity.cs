using System;
using System.Collections.Generic;
using System.Text;

namespace PicoShelter_ApiServer.DAL.Interfaces
{
    public interface IEntity
    {
        public int Id { get; set; }
        public DateTime CreatedDateUTC { get; set; }
    }
}
