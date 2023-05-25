using PicoShelter_ApiServer.DAL.Abstract;
using PicoShelter_ApiServer.DAL.EF;
using PicoShelter_ApiServer.DAL.Entities;

namespace PicoShelter_ApiServer.DAL.Repositories
{
    public class ImageLikesRepository : RepositoryBase<ImageLikeEntity>
    {
        public ImageLikesRepository(ApplicationContext context) : base(context)
        {
        }
    }
}