using PicoShelter_ApiServer.DAL.Abstract;
using PicoShelter_ApiServer.DAL.EF;
using PicoShelter_ApiServer.DAL.Entities;

namespace PicoShelter_ApiServer.DAL.Repositories
{
    public class AlbumImagesRepository : RepositoryBase<AlbumImageEntity>
    {
        public AlbumImagesRepository(ApplicationContext context) : base(context)
        {
        }
    }
}
