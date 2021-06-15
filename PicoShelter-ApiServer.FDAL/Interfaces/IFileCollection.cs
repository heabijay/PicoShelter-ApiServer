using System.Collections.Generic;
using System.IO;

namespace PicoShelter_ApiServer.FDAL.Interfaces
{
    public interface IFileCollection<T> : ICollection<T> where T : IFileEntity
    {
        public string BasePath { get; set; }
        public string GetFullPath(T item);
        public Stream CreateOrUpdate(T item);
        public Stream Get(T item);
    }
}
