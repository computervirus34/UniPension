using UniPension.Data;
using UniPension.IRepositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace UniPension.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected UniPensionDBEntities  _context;
        protected DbSet<T> _dbSet;
        public GenericRepository(
            UniPensionDBEntities context
            )
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public virtual T GetByID(int id)
        {
            return _dbSet.Find(id);
        }

        public virtual bool Insert(T entity)
        {
            _dbSet.Add(entity);
            return true;
        }

        public virtual bool Delete(int id)
        {
            T entityToDelete = _dbSet.Find(id);
            Delete(entityToDelete);
            return true;
        }

        public virtual void Delete(T entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
        }

        public virtual bool Update(T entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
            return true;
        }
    }
}