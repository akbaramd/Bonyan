using System.Collections.Concurrent;
using System.Linq.Expressions;
using Bonyan.DomainDrivenDesign.Domain.Abstractions;
using Bonyan.DomainDrivenDesign.Domain.Entities;
using Bonyan.DomainDrivenDesign.Domain.Specifications;

namespace Bonyan.AspNetCore.Persistence;

public class InMemoryReadOnlyRepository<TEntity> : IReadOnlyRepository<TEntity>
    where TEntity : class, IEntity
{
    protected readonly ConcurrentDictionary<object, TEntity> _store = new();
    public ITenantAccessor? TenantAccessor { get; set; }

    public InMemoryReadOnlyRepository(IServiceProvider serviceProvider)
    {
        TenantAccessor = serviceProvider.GetService<ITenantAccessor>();
    }

    public Task<IEnumerable<TEntity>> GetAllAsync()
    {
        var result = _store.Values.AsEnumerable();
        return Task.FromResult(result);
    }

    public Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var compiledPredicate = predicate.Compile();
        var result = _store.Values.Where(compiledPredicate);
        return Task.FromResult(result.AsEnumerable());
    }

    public Task<IEnumerable<TEntity>> FindAsync(Specification<TEntity> specification)
    {
        var compiledPredicate = specification.ToExpression().Compile();
        var result = _store.Values.Where(compiledPredicate);
        return Task.FromResult(result.AsEnumerable());
    }

    public Task<TEntity?> GetSingleByPredicateAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var compiledPredicate = predicate.Compile();
        var result = _store.Values.SingleOrDefault(compiledPredicate);
        return Task.FromResult(result);
    }

    public Task<TEntity?> GetFirstByPredicateAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var compiledPredicate = predicate.Compile();
        var result = _store.Values.FirstOrDefault(compiledPredicate);
        return Task.FromResult(result);
    }

    public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var compiledPredicate = predicate.Compile();
        var count = _store.Values.Count(compiledPredicate);
        return Task.FromResult(count);
    }

    public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var compiledPredicate = predicate.Compile();
        var exists = _store.Values.Any(compiledPredicate);
        return Task.FromResult(exists);
    }

    public bool? IsChangeTrackingEnabled { get; }
    
    
}

public class InMemoryReadOnlyRepository<TEntity, TKey> : InMemoryReadOnlyRepository<TEntity>, IReadOnlyRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey> where TKey : notnull
{
    public InMemoryReadOnlyRepository(IServiceProvider serviceProvider) : base(serviceProvider) { }

    public Task<TEntity?> GetByIdAsync(TKey id)
    {
        _store.TryGetValue(id ?? throw new ArgumentNullException(nameof(id)), out var entity);
        return Task.FromResult(entity);
    }

    public Task<IEnumerable<TEntity>> GetByPredicateAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var compiledPredicate = predicate.Compile();
        var result = _store.Values.Where(compiledPredicate);
        return Task.FromResult(result.AsEnumerable());
    }
}
