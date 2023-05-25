using PicoShelter_ApiServer.DAL.Abstract;
using PicoShelter_ApiServer.DAL.EF;
using PicoShelter_ApiServer.DAL.Entities;

namespace PicoShelter_ApiServer.DAL.Repositories
{
    public class BansRepository : RepositoryBase<BanEntity>
    {
        public BansRepository(ApplicationContext context) : base(context)
        {
        }
    }
}
