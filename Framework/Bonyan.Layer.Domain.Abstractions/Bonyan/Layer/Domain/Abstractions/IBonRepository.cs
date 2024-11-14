using System.Linq.Expressions;
using Bonyan.Layer.Domain.Abstractions.Results;
using Bonyan.MultiTenant;
using Bonyan.UnitOfWork;

namespace Bonyan.Layer.Domain.Abstractions;

public interface IBonRepository : IBonUnitOfWorkEnabled
{
    bool? IsChangeTrackingEnabled { get; }
    public IBonCurrentTenant? CurrentTenant { get; }
}

public interface IBonReadOnlyRepository<TEntity, in TKey> : IBonReadOnlyRepository<TEntity>
    where TEntity : class, IBonEntity<TKey> where TKey : notnull
{
    Task<TEntity?> FindByIdAsync(TKey id);
    Task<TEntity?> GetByIdAsync(TKey id);
    Task<IEnumerable<TEntity>> GetByPredicateAsync(Expression<Func<TEntity, bool>> predicate);
}

// IReadOnlyRepository without specifying the key type (TKesdasdasy is assumed internally by the entity).
public interface IBonReadOnlyRepository<TEntity> : IBonRepository where TEntity : class, IBonEntity
{
    public IQueryable<TEntity> Queryable { get; }
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> predicate);
    Task<IEnumerable<TEntity>> FindAsync(IBonSpecification<TEntity> bonSpecification);
    Task<TEntity?> FindOneAsync(IBonSpecification<TEntity> bonSpecification);
    Task<TEntity> GetOneAsync(IBonSpecification<TEntity> bonSpecification);
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
    
    Task<BonPaginatedResult<TEntity>> PaginatedAsync(BonPaginatedSpecification<TEntity> paginateSpecification);
    Task<BonPaginatedResult<TEntity>> PaginatedAsync(BonPaginatedAndSortableSpecification<TEntity> paginateSpecification);
    Task<BonPaginatedResult<TEntity>> PaginatedAsync(Expression<Func<TEntity, bool>> predicate, int take, int skip);
}

// IRepository for full CRUD operations, extending IReadOnlyRepository.
public interface IBonRepository<TEntity, in TKey> : IBonRepository<TEntity>, IBonReadOnlyRepository<TEntity, TKey>
    where TEntity : class, IBonEntity<TKey> where TKey : notnull
{
    Task DeleteByIdAsync(TKey id, bool autoSave = false);
}

// IRepository without specifying the key type (uses IEntity directly).
public interface IBonRepository<TEntity> : IBonReadOnlyRepository<TEntity>, IBonUnitOfWorkEnabled where TEntity : class, IBonEntity
{
    Task<TEntity> AddAsync(TEntity entity, bool autoSave = false);
    Task UpdateAsync(TEntity entity, bool autoSave = false);
    Task DeleteAsync(TEntity entity, bool autoSave = false);
    
    // Bulk operations
    Task AddRangeAsync(IEnumerable<TEntity> entities, bool autoSave = false);
    Task UpdateRangeAsync(IEnumerable<TEntity> entities, bool autoSave = false);
    Task DeleteRangeAsync(IEnumerable<TEntity> entities, bool autoSave = false);
}