using System.Linq.Expressions;
using Bonyan.DomainDrivenDesign.Domain.Entities;
using Bonyan.DomainDrivenDesign.Domain.Specifications;

namespace Bonyan.DomainDrivenDesign.Domain.Abstractions;

public interface IRepository
{
  bool? IsChangeTrackingEnabled { get; }
}
public interface IReadOnlyRepository<TEntity, in TKey> : IReadOnlyRepository<TEntity> where TEntity : class, IEntity<TKey> where TKey : notnull
{
  Task<TEntity?> GetByIdAsync(TKey id);
  Task<IEnumerable<TEntity>> GetByPredicateAsync(Expression<Func<TEntity, bool>> predicate);
}

// IReadOnlyRepository without specifying the key type (TKesdasdasy is assumed internally by the entity).
public interface IReadOnlyRepository<TEntity> : IRepository where TEntity : class, IEntity
{
  Task<IEnumerable<TEntity>> GetAllAsync();
  Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
  Task<IEnumerable<TEntity>> FindAsync(Specification<TEntity> specification);
  Task<TEntity?> GetSingleByPredicateAsync(Expression<Func<TEntity, bool>> predicate);
  Task<TEntity?> GetFirstByPredicateAsync(Expression<Func<TEntity, bool>> predicate);
  Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
  Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
}

// IRepository for full CRUD operations, extending IReadOnlyRepository.
public interface IRepository<TEntity, in TKey> :IRepository<TEntity>, IReadOnlyRepository<TEntity, TKey> where TEntity : class, IEntity<TKey> where TKey : notnull
{
  Task DeleteByIdAsync(TKey id);
}

// IRepository without specifying the key type (uses IEntity directly).
public interface IRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : class, IEntity
{
  Task<TEntity> AddAsync(TEntity entity);
  Task UpdateAsync(TEntity entity);
  Task DeleteAsync(TEntity entity);
}

