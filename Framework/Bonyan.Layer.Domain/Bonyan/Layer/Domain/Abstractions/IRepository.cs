using System.Linq.Expressions;
using Bonyan.Layer.Domain.Entities;
using Bonyan.Layer.Domain.Model;
using Bonyan.Layer.Domain.Specifications;

namespace Bonyan.Layer.Domain.Abstractions;

public interface IRepository
{
  bool? IsChangeTrackingEnabled { get; }
}
public interface IReadOnlyRepository<TEntity, in TKey> : IReadOnlyRepository<TEntity> where TEntity : class, IEntity<TKey> where TKey : notnull
{
  Task<TEntity?> FindByIdAsync(TKey id);
  Task<TEntity?> GetByIdAsync(TKey id);
  Task<IEnumerable<TEntity>> GetByPredicateAsync(Expression<Func<TEntity, bool>> predicate);
}

// IReadOnlyRepository without specifying the key type (TKesdasdasy is assumed internally by the entity).
public interface IReadOnlyRepository<TEntity> : IRepository where TEntity : class, IEntity
{

  Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
  Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> predicate);
  Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> predicate);
  Task<IEnumerable<TEntity>> FindAsync(ISpecification<TEntity> specification);
  Task<TEntity?> FindOneAsync(ISpecification<TEntity> specification);
  Task<TEntity> GetOneAsync(ISpecification<TEntity> specification);
  Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
  Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
  Task<PaginatedResult<TEntity>> PaginatedAsync(PaginatedSpecification<TEntity> paginateSpecification);
  Task<PaginatedResult<TEntity>> PaginatedAsync(PaginatedAndSortableSpecification<TEntity> paginateSpecification);
  Task<PaginatedResult<TEntity>> PaginatedAsync(Expression<Func<TEntity, bool>> predicate,int take , int skip);
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

