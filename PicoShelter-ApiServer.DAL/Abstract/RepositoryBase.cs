using PicoShelter_ApiServer.DAL.EF;
using PicoShelter_ApiServer.DAL.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace PicoShelter_ApiServer.DAL.Abstract
{
    public abstract class RepositoryBase<T> : IRepository<T> where T : class, IEntity
    {
        private protected readonly ApplicationContext _db;
        private protected readonly static object _locker = new();

        public RepositoryBase(ApplicationContext context)
        {
            _db = context;
        }

        public virtual void Add(T item)
        {
            item.CreatedDateUTC = DateTime.UtcNow;
            lock (_locker)
            {
                _db.Set<T>().Add(item);
            }
        }
        public virtual bool Any(Func<T, bool> predicate)
        {
            return _db.Set<T>().Any(predicate);
        }
        public virtual void Delete(int id)
        {
            lock (_locker)
            {
                var acc = _db.Set<T>().Find(id);
                if (acc != null)
                    _db.Set<T>().Remove(acc);
            }
        }

        public virtual T FirstOrDefault(Func<T, bool> predicate)
        {
            return _db.Set<T>().FirstOrDefault(predicate);
        }

        public virtual T Get(int id)
        {
            return _db.Set<T>().Find(id);
        }

        public virtual IQueryable<T> GetAll()
        {
            return _db.Set<T>().AsQueryable();
        }

        public virtual void Update(T item)
        {
            lock (_locker)
            {
                _db.Set<T>().Update(item);
            }
        }

        public virtual IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return GetAll().Where(predicate);
        }
    }
}
