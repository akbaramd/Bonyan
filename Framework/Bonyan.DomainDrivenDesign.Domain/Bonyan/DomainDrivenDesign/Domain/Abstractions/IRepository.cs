using System.Linq.Expressions;
using Bonyan.DomainDrivenDesign.Domain.Entities;
using Bonyan.DomainDrivenDesign.Domain.Model;
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
  Task<TEntity?> FindByIdAsync(Guid id);
  Task<TEntity> GetByIdAsync(Guid id);
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

