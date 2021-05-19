using System;
using System.Collections.Generic;

namespace PicoShelter_ApiServer.DAL.Interfaces
{
    public interface IRepository<T> where T : IEntity
    {
        public void Add(T item);
        public T Get(int id);
        public IEnumerable<T> GetAll();
        public void Update(T item);
        public void Delete(int id);
        public bool Any(Func<T, bool> predicate);
        public T FirstOrDefault(Func<T, bool> predicate);
        public IEnumerable<T> Where(Func<T, bool> predicate);
    }
}
