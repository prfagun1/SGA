using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using SGA.Interfaces;
using SGA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SGA.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private SGAContext _context = null;
        DbSet<T> _dbSet;

        public Repository(SGAContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public int Count()
        {

            return _dbSet.Count();
        }

        public void Create(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public T Get(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query.Where(filter).FirstOrDefault();
        }


        public IQueryable<T> GetList(List<Expression<Func<T, bool>>> filter = null, params Expression<Func<T, object>>[] includeProperties)
        {

            IQueryable<T> query = _dbSet.AsQueryable();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }


            if (filter != null)
            {
                foreach (var filterValue in filter)
                {
                    query = query.Where(filterValue);
                }
            }

            return query;

        }


        public IQueryable<T> GetList(List<Expression<Func<T, bool>>> filter = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {

            IQueryable<T> query = _dbSet.AsQueryable();

            if (include != null)
            {
                query = include(query);
            }


            if (filter != null)
            {
                foreach (var filterValue in filter)
                {
                    query = query.Where(filterValue);
                }
            }

            return query;

        }

        public void SQLCommand(string sql)
        {
            _context.Database.ExecuteSqlCommand(sql);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Reload() {

            foreach (var entity in _context.ChangeTracker.Entries())
            {
                entity.Reload();
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }

    }
}
