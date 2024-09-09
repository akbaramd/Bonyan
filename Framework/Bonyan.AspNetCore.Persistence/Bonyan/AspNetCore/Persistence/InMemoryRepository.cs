using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bonyan.DomainDrivenDesign.Domain.Abstractions;
using Bonyan.DomainDrivenDesign.Domain.Entities;
using Bonyan.DomainDrivenDesign.Domain.Specifications;

namespace Bonyan.AspNetCore.Persistence
{
    // In-memory repository for entities without a specific key type (uses GetKeys)
    public class InMemoryRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        // Single store for all TEntity instances, keyed by their generated composite key
        protected readonly ConcurrentDictionary<object, TEntity> _store = new ConcurrentDictionary<object, TEntity>();

        private object GetEntityKey(TEntity entity)
        {
            var keys = entity.GetKeys();
            if (keys == null || keys.Length == 0)
                throw new InvalidOperationException("Entity does not have valid keys.");

            return string.Join("-", keys); // Generate composite key as a string
        }

        public Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return Task.FromResult(_store.Values.AsEnumerable());
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

        public Task<TEntity> AddAsync(TEntity entity)
        {
            var key = GetEntityKey(entity);

            if (!_store.TryAdd(key, entity))
            {
                throw new InvalidOperationException("Entity with the same key already exists.");
            }

            return Task.FromResult(entity);
        }

        public Task UpdateAsync(TEntity entity)
        {
            var key = GetEntityKey(entity);

            if (!_store.ContainsKey(key))
            {
                throw new KeyNotFoundException("Entity not found for update.");
            }

            _store[key] = entity;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(TEntity entity)
        {
            var key = GetEntityKey(entity);

            if (!_store.TryRemove(key, out _))
            {
                throw new KeyNotFoundException("Entity not found for deletion.");
            }

            return Task.CompletedTask;
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
    }

    // In-memory repository for entities with a defined key type (TKey)
    public class InMemoryRepository<TEntity, TKey> : InMemoryRepository<TEntity>, IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey> where TKey : notnull
    {
        // No need for an additional store, as we already have _store from the base class

        public InMemoryRepository()
        {
        }


        public Task<TEntity?> GetByIdAsync(TKey id)
        {
            _store.TryGetValue(id, out var entity);
            return Task.FromResult(entity);
        }

        public Task DeleteByIdAsync(TKey id)
        {
            if (!_store.TryRemove(id, out _))
            {
                throw new KeyNotFoundException("Entity not found for deletion.");
            }

            return Task.CompletedTask;
        }

        public new Task<TEntity> AddAsync(TEntity entity)
        {
            var key = entity.Id;

            if (!_store.TryAdd(entity.Id, entity))
            {
                throw new InvalidOperationException("Entity with the same key already exists.");
            }

            return Task.FromResult(entity);
        }

        public new Task UpdateAsync(TEntity entity)
        {
            var key = entity.Id;

            if (!_store.ContainsKey(entity.Id))
            {
                throw new KeyNotFoundException("Entity not found for update.");
            }

            _store[entity.Id] = entity;
            return Task.CompletedTask;
        }

        public new Task DeleteAsync(TEntity entity)
        {
            var key = entity.Id;

            if (!_store.TryRemove(entity.Id, out _))
            {
                throw new KeyNotFoundException("Entity not found for deletion.");
            }

            return Task.CompletedTask;
        }

        public Task<IEnumerable<TEntity>> GetByPredicateAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var compiledPredicate = predicate.Compile();
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
    }
}
