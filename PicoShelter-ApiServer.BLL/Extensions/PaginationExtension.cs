﻿using System.Linq;

namespace PicoShelter_ApiServer.BLL.Extensions
{
    public static class PaginationExtension
    {
        public static IQueryable<T> Pagination<T>(this IQueryable<T> collection, int? starts, int? count, out int summaryCount)
        {
            summaryCount = collection.Count();

            if (starts != null)
                collection = collection.Skip(starts.Value);

            if (count != null)
                collection = collection.Take(count.Value);

            return collection;
        }
    }
}
