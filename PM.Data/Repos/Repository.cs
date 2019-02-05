using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PM.Data.Repos
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext Context;

        public Repository(DbContext _context)
        {
            Context = _context;
        }

        public T Create(T entity)
        {
            try
            {
                Context.Set<T>().AddOrUpdate(entity);
                Context.SaveChanges();
                Context.Entry<T>(entity).Reload();
                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Delete(T entity)
        {
            try
            {
                Context.Set<T>().Remove(entity);
                return Context.SaveChanges() != 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<T> GetAll()
        {
            return Context.Set<T>().AsEnumerable();
        }

        public virtual T GetById(object identifier)
        {
            return Context.Set<T>().Find(identifier);
        }

        public IEnumerable<T> Search(Expression<Func<T, bool>> query)
        {
            return Context.Set<T>().Where(query);
        }

        public bool Update(T entity)
        {
            try
            {
                //Context.Set<T>().Attach(entity);
                //Context.Entry(entity).State = EntityState.Modified;

                Context.Set<T>().AddOrUpdate(entity);
                //Context.Entry(entity).CurrentValues.SetValues(entity);
                return Context.SaveChanges() != 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
