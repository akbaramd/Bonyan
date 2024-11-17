using System.Linq.Expressions;
using Bonyan.Layer.Domain.Entity;
using Bonyan.Layer.Domain.Specification.Abstractions;

namespace Bonyan.Layer.Domain.Repository.Abstractions;

public interface IBonReadOnlyRepository<TEntity> : IBonRepository where TEntity : class, IBonEntity
{
    public Task<IQueryable<TEntity>> GetQueryableAsync();
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

public interface IBonReadOnlyRepository<TEntity, in TKey> : IBonReadOnlyRepository<TEntity>
    where TEntity : class, IBonEntity<TKey>
{
    Task<TEntity?> FindByIdAsync(TKey id);
    Task<TEntity?> GetByIdAsync(TKey id);
    Task<IEnumerable<TEntity>> GetByPredicateAsync(Expression<Func<TEntity, bool>> predicate);
}