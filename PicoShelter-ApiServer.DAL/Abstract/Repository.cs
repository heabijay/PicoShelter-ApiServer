using PicoShelter_ApiServer.DAL.EF;
using PicoShelter_ApiServer.DAL.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace PicoShelter_ApiServer.DAL.Abstract
{
    public abstract class Repository<T> : IRepository<T> where T : class, IEntity
    {
        protected ApplicationContext db;
        protected static object locker = new object();
        public Repository(ApplicationContext context)
        {
            db = context;
        }

        public virtual void Add(T item)
        {
            item.CreatedDateUTC = DateTime.UtcNow;
            lock (locker)
            {
                db.Set<T>().Add(item);
            }
        }
        public virtual bool Any(Func<T, bool> predicate)
        {
            return db.Set<T>().Any(predicate);
        }
        public virtual void Delete(int id)
        {
            lock (locker)
            {
                var acc = db.Set<T>().Find(id);
                if (acc != null)
                    db.Set<T>().Remove(acc);
            }
        }

        public virtual T FirstOrDefault(Func<T, bool> predicate)
        {
            return db.Set<T>().FirstOrDefault(predicate);
        }

        public virtual T Get(int id)
        {
            return db.Set<T>().Find(id);
        }

        public virtual IQueryable<T> GetAll()
        {
            return db.Set<T>().AsQueryable();
        }

        public virtual void Update(T item)
        {
            lock (locker)
            {
                db.Set<T>().Update(item);
            }
        }

        public virtual IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return GetAll().Where(predicate);
        }
    }
}
