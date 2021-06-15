using PicoShelter_ApiServer.DAL.Abstract;
using PicoShelter_ApiServer.DAL.EF;
using PicoShelter_ApiServer.DAL.Entities;

namespace PicoShelter_ApiServer.DAL.Repositories
{
    public class ProfilesRepository : RepositoryBase<ProfileEntity>
    {
        public ProfilesRepository(ApplicationContext context) : base(context)
        {
        }
    }
}
