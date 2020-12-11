using Microsoft.EntityFrameworkCore;
using PicoShelter_ApiServer.DAL.Abstract;
using PicoShelter_ApiServer.DAL.EF;
using PicoShelter_ApiServer.DAL.Entities;
using PicoShelter_ApiServer.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PicoShelter_ApiServer.DAL.Repositories
{
    public class AccountsRepository : Repository<AccountEntity>
    {
        public AccountsRepository(ApplicationContext context) : base(context)
        {
        }
    }
}
