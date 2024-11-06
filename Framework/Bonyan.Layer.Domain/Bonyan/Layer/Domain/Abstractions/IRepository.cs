using System.Linq.Expressions;
using Bonyan.Layer.Domain.Entities;
using Bonyan.Layer.Domain.Model;
using Bonyan.Layer.Domain.Specifications;
using Bonyan.MultiTenant;
using Bonyan.UnitOfWork;

namespace Bonyan.Layer.Domain.Abstractions;

public interface IRepository : IUnitOfWorkEnabled
{
    bool? IsChangeTrackingEnabled { get; }
    public ICurrentTenant? CurrentTenant { get; }
}

public interface IReadOnlyRepository<TEntity, in TKey> : IReadOnlyRepository<TEntity>
    where TEntity : class, IEntity<TKey> where TKey : notnull
{
    Task<TEntity?> FindByIdAsync(TKey id);
    Task<TEntity?> GetByIdAsync(TKey id);
    Task<IEnumerable<TEntity>> GetByPredicateAsync(Expression<Func<TEntity, bool>> predicate);
}

// IReadOnlyRepository without specifying the key type (TKesdasdasy is assumed internally by the entity).
public interface IReadOnlyRepository<TEntity> : IRepository where TEntity : class, IEntity
{
    public IQueryable<TEntity> Queryable { get; }
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
    Task<PaginatedResult<TEntity>> PaginatedAsync(Expression<Func<TEntity, bool>> predicate, int take, int skip);
}

// IRepository for full CRUD operations, extending IReadOnlyRepository.
public interface IRepository<TEntity, in TKey> : IRepository<TEntity>, IReadOnlyRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey> where TKey : notnull
{
    Task DeleteByIdAsync(TKey id, bool autoSave = false);
}

// IRepository without specifying the key type (uses IEntity directly).
public interface IRepository<TEntity> : IReadOnlyRepository<TEntity>, IUnitOfWorkEnabled where TEntity : class, IEntity
{
    Task<TEntity> AddAsync(TEntity entity, bool autoSave = false);
    Task UpdateAsync(TEntity entity, bool autoSave = false);
    Task DeleteAsync(TEntity entity, bool autoSave = false);
}