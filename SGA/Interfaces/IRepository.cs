using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Query;
using SGA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SGA.Interfaces
{
    public interface IRepository<T> where T : class
    {
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
        T Get(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includeProperties);
        IQueryable<T> GetList(List<Expression<Func<T, bool>>> filter = null, params Expression<Func<T, object>>[] includeProperties);
        IQueryable<T> GetList(List<Expression<Func<T, bool>>> filter = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);
        int Count();
        void SQLCommand(string sql);
        void Reload();


    }
    
}
