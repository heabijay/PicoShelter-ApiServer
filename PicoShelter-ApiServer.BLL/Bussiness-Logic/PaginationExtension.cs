using System.Collections.Generic;
using System.Linq;

namespace PicoShelter_ApiServer.BLL.Bussiness_Logic
{
    public static class PaginationExtension
    {
        public static IEnumerable<T> Pagination<T>(this IEnumerable<T> collection, int? starts, int? count)
        {
            if (starts != null)
                collection = collection.Skip(starts.Value);

            if (count != null)
                collection = collection.Take(count.Value);

            return collection;
        }
    }
}
