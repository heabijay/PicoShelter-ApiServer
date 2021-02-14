﻿using PicoShelter_ApiServer.DAL.Abstract;
using PicoShelter_ApiServer.DAL.EF;
using PicoShelter_ApiServer.DAL.Entities;
using System;

namespace PicoShelter_ApiServer.DAL.Repositories
{
    public class ImagesRepository : Repository<ImageEntity>
    {
        public ImagesRepository(ApplicationContext context) : base(context)
        {
        }
    }
}
