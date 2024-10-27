using Bonyan.DomainDrivenDesign.Domain.Abstractions;
using Bonyan.DomainDrivenDesign.Domain.Entities;

namespace Bonyan.AspNetCore.Persistence;

public class InMemoryRepository<TEntity> : InMemoryReadOnlyRepository<TEntity>, IRepository<TEntity>
    where TEntity : class, IEntity 
{
    public InMemoryRepository(IServiceProvider serviceProvider) : base(serviceProvider) { }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
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

        await Task.CompletedTask;
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
}

public class InMemoryRepository<TEntity, TKey> : InMemoryReadOnlyRepository<TEntity, TKey>, IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey> where TKey : notnull
{
    public InMemoryRepository(IServiceProvider serviceProvider) : base(serviceProvider) { }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        var key = entity.Id;
        if (!_store.TryAdd(key ?? throw new InvalidOperationException(), entity))
        {
            throw new InvalidOperationException("Entity with the same key already exists.");
        }

        return await Task.FromResult(entity);
    }

    public  async Task UpdateAsync(TEntity entity)
    {
        var key = entity.Id;
        if (!_store.ContainsKey(key))
        {
            throw new KeyNotFoundException("Entity not found for update.");
        }

        _store[key] = entity;
        await Task.CompletedTask;
    }

    public  async Task DeleteAsync(TEntity entity)
    {
        var key = entity.Id;
        if (!_store.TryRemove(key, out _))
        {
            throw new KeyNotFoundException("Entity not found for deletion.");
        }

        await Task.CompletedTask;
    }

    public async Task DeleteByIdAsync(TKey id)
    {
        if (!_store.TryRemove(id, out _))
        {
            throw new KeyNotFoundException("Entity not found for deletion.");
        }

        await Task.CompletedTask;
    }
}
