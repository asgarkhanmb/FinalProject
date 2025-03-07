﻿using Domain.Common;
using Domain.Entities;
using System.Linq.Expressions;

namespace Repository.Repositories.Interfaces
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task<int> SaveChanges();
        Task CreateAsync(T entity);
        Task EditAsync(T entity);
        Task<T> GetByInclude(Expression<Func<T, bool>> predicate, params string[] includes);
        IQueryable<T> FindAllWithIncludes();
        Task DeleteAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetById(int id);
        Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> predicate);
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<bool> ExistAsync(Expression<Func<T, bool>> predicate);
    }
}
