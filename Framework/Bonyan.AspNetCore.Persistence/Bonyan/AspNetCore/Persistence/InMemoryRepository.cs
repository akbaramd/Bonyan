using System.Collections.Concurrent;
using System.Linq.Expressions;
using Bonyan.DomainDrivenDesign.Domain.Abstractions;
using Bonyan.DomainDrivenDesign.Domain.Entities;
using Bonyan.DomainDrivenDesign.Domain.Specifications;

namespace Bonyan.AspNetCore.Persistence;

public class InMemoryRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
{
  protected readonly ConcurrentDictionary<object, TEntity> _store = new();

  public InMemoryRepository(IServiceProvider serviceProvider)
  {
    TenantAccessor = serviceProvider.GetService<ITenantAccessor>();
  }

  public ITenantAccessor? TenantAccessor { get; set; }

  public Task<IEnumerable<TEntity>> GetAllAsync()
  {
    // Return all entities that the current tenant has access to
    var result = _store.Values.Where(IsEntityAccessibleByTenant);
    return Task.FromResult(result.AsEnumerable());
  }

  public Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
  {
    var compiledPredicate = predicate.Compile();
    var result = _store.Values.Where(compiledPredicate).Where(IsEntityAccessibleByTenant);
    return Task.FromResult(result.AsEnumerable());
  }

  public Task<IEnumerable<TEntity>> FindAsync(Specification<TEntity> specification)
  {
    var compiledPredicate = specification.ToExpression().Compile();
    var result = _store.Values.Where(compiledPredicate).Where(IsEntityAccessibleByTenant);
    return Task.FromResult(result.AsEnumerable());
  }

  public async Task<TEntity> AddAsync(TEntity entity)
  {
    // Ensure the current tenant is allowed to add this entity
    if (!IsEntityAccessibleByTenant(entity))
    {
      throw new UnauthorizedAccessException("Tenant does not have access to add this entity.");
    }

    var key = GetEntityKey(entity);
    if (!_store.TryAdd(key, entity))
    {
      throw new InvalidOperationException("Entity with the same key already exists.");
    }

    return await Task.FromResult(entity);
  }

  public async Task UpdateAsync(TEntity entity)
  {
    var key = GetEntityKey(entity);

    if (!_store.ContainsKey(key))
    {
      throw new KeyNotFoundException("Entity not found for update.");
    }

    // Ensure tenant access for update
    if (!IsEntityAccessibleByTenant(entity))
    {
      throw new UnauthorizedAccessException("Tenant does not have access to update this entity.");
    }

    _store[key] = entity;
    await Task.CompletedTask;
  }

  public async Task DeleteAsync(TEntity entity)
  {
    var key = GetEntityKey(entity);

    if (!_store.TryRemove(key, out _))
    {
      throw new KeyNotFoundException("Entity not found for deletion.");
    }

    // Ensure tenant access for deletion
    if (!IsEntityAccessibleByTenant(entity))
    {
      throw new UnauthorizedAccessException("Tenant does not have access to delete this entity.");
    }

    await Task.CompletedTask;
  }

  public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
  {
    var compiledPredicate = predicate.Compile();
    var count = _store.Values.Where(IsEntityAccessibleByTenant).Count(compiledPredicate);
    return Task.FromResult(count);
  }

  public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
  {
    var compiledPredicate = predicate.Compile();
    var exists = _store.Values.Where(IsEntityAccessibleByTenant).Any(compiledPredicate);
    return Task.FromResult(exists);
  }

  private object GetEntityKey(TEntity entity)
  {
    var keys = entity.GetKeys();
    if (keys == null || keys.Length == 0)
    {
      throw new InvalidOperationException("Entity does not have valid keys.");
    }

    return string.Join("-", keys);
  }

  public bool IsEntityAccessibleByTenant(TEntity? entity)
  {
    if (entity == null)
    {
      return false;
    }

    if (entity is not ITenant tenantEntity)
    {
      return true; // Entity is not tenant-specific, no restrictions
    }

    var currentTenant = TenantAccessor?.CurrentTenant;
    if (currentTenant == null)
    {
      return true; // No tenant restrictions, allow access
    }

    return currentTenant.Contains(tenantEntity.Tenant);
  }
}

public class InMemoryRepository<TEntity, TKey> : InMemoryRepository<TEntity>, IRepository<TEntity, TKey>
  where TEntity : class, IEntity<TKey>
  where TKey : notnull
{
  public InMemoryRepository(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }

  public Task<TEntity?> GetByIdAsync(TKey id)
  {
    _store.TryGetValue(id, out var entity);

    if (IsEntityAccessibleByTenant(entity))
    {
      throw new UnauthorizedAccessException("Tenant does not have access to this entity.");
    }

    return Task.FromResult(entity);
  }

  public Task DeleteByIdAsync(TKey id)
  {
    if (!_store.TryRemove(id, out var entity))
    {
      throw new KeyNotFoundException("Entity not found for deletion.");
    }

    if (!IsEntityAccessibleByTenant(entity))
    {
      throw new UnauthorizedAccessException("Tenant does not have access to delete this entity.");
    }

    return Task.CompletedTask;
  }

  public new Task<TEntity> AddAsync(TEntity entity)
  {
    // Ensure tenant access for addition
    if (!IsEntityAccessibleByTenant(entity))
    {
      throw new UnauthorizedAccessException("Tenant does not have access to add this entity.");
    }

    var key = entity.Id;

    if (!_store.TryAdd(key, entity))
    {
      throw new InvalidOperationException("Entity with the same key already exists.");
    }

    return Task.FromResult(entity);
  }

  public new Task UpdateAsync(TEntity entity)
  {
    var key = entity.Id;

    if (!_store.ContainsKey(key))
    {
      throw new KeyNotFoundException("Entity not found for update.");
    }

    // Ensure tenant access for update
    if (!IsEntityAccessibleByTenant(entity))
    {
      throw new UnauthorizedAccessException("Tenant does not have access to update this entity.");
    }

    _store[key] = entity;
    return Task.CompletedTask;
  }

  public new Task DeleteAsync(TEntity entity)
  {
    var key = entity.Id;

    if (!_store.TryRemove(key, out _))
    {
      throw new KeyNotFoundException("Entity not found for deletion.");
    }

    // Ensure tenant access for deletion
    if (!IsEntityAccessibleByTenant(entity))
    {
      throw new UnauthorizedAccessException("Tenant does not have access to delete this entity.");
    }

    return Task.CompletedTask;
  }

  public Task<IEnumerable<TEntity>> GetByPredicateAsync(Expression<Func<TEntity, bool>> predicate)
  {
    var compiledPredicate = predicate.Compile();
    var result = _store.Values.Where(compiledPredicate).Where(IsEntityAccessibleByTenant);
    return Task.FromResult(result.AsEnumerable());
  }

  public Task<TEntity?> GetSingleByPredicateAsync(Expression<Func<TEntity, bool>> predicate)
  {
    var compiledPredicate = predicate.Compile();
    var result = _store.Values.SingleOrDefault(compiledPredicate);

    if (!IsEntityAccessibleByTenant(result))
    {
      throw new UnauthorizedAccessException("Tenant does not have access to this entity.");
    }

    return Task.FromResult(result);
  }

  public Task<TEntity?> GetFirstByPredicateAsync(Expression<Func<TEntity, bool>> predicate)
  {
    var compiledPredicate = predicate.Compile();
    var result = _store.Values.FirstOrDefault(compiledPredicate);

    if (!IsEntityAccessibleByTenant(result))
    {
      throw new UnauthorizedAccessException("Tenant does not have access to this entity.");
    }

    return Task.FromResult(result);
  }
}
