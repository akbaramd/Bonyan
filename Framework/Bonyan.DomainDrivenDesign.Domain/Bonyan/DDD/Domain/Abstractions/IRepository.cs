using System.Linq.Expressions;
using Bonyan.DDD.Domain.Entities;
using Bonyan.DDD.Domain.Specifications;

namespace Bonyan.DDD.Domain.Abstractions;

public interface IRepository<TEntity, in TKey> : IRepository<TEntity> where TEntity : class, IEntity
{
    Task<TEntity?> GetByIdAsync(TKey id);
    Task DeleteByIdAsync(TKey id);
    Task<IEnumerable<TEntity>> GetByPredicateAsync(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity?> GetSingleByPredicateAsync(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity?> GetFirstByPredicateAsync(Expression<Func<TEntity, bool>> predicate);
}

public interface IRepository<TEntity> : IRepository where TEntity : class, IEntity
{
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    Task<IEnumerable<TEntity>> FindAsync(Specification<TEntity> specification);
    Task<TEntity> AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
}


public interface IRepository
{
  
}