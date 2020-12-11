﻿using PicoShelter_ApiServer.DAL.Interfaces;
using System;

namespace PicoShelter_ApiServer.DAL.Abstract
{
    public abstract class Entity : IEntity
    {
        public int Id { get; set; }
        public DateTime CreatedDateUTC { get; set; }
    }
}
