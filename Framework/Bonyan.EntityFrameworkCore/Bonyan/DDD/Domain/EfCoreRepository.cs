using System.Linq.Expressions;
using Bonyan.DDD.Domain.Abstractions;
using Bonyan.DDD.Domain.Entities;
using Bonyan.DDD.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Bonyan.DDD.Domain;

  public class EfCoreRepository<TEntity, TDbContext> : IRepository<TEntity>
      where TEntity : class, IEntity where TDbContext : DbContext
  {
      protected readonly TDbContext _dbContext;
      protected readonly DbSet<TEntity> _dbSet;

      public EfCoreRepository(TDbContext dbContext)
      {
          _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
          _dbSet = _dbContext.Set<TEntity>();
      }

      public async Task<IEnumerable<TEntity>> GetAllAsync()
      {
          return await _dbSet.ToListAsync();
      }

      public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
      {
          return await _dbSet.Where(predicate).ToListAsync();
      }

      public async Task<IEnumerable<TEntity>> FindAsync(Specification<TEntity> specification)
      {
          var query = specification.Apply(_dbSet.AsQueryable());
          return await query.ToListAsync();
      }

      public async Task<TEntity> AddAsync(TEntity entity)
      {
          await _dbSet.AddAsync(entity);
          await _dbContext.SaveChangesAsync();
          return entity;
      }

      public async Task UpdateAsync(TEntity entity)
      {
          _dbSet.Update(entity);
          await _dbContext.SaveChangesAsync();
      }

      public async Task DeleteAsync(TEntity entity)
      {
          _dbSet.Remove(entity);
          await _dbContext.SaveChangesAsync();
      }

      public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
      {
          return await _dbSet.CountAsync(predicate);
      }

      public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
      {
          return await _dbSet.AnyAsync(predicate);
      }

      public async Task<TEntity?> GetSingleByPredicateAsync(Expression<Func<TEntity, bool>> predicate)
      {
          return await _dbSet.SingleOrDefaultAsync(predicate);
      }

      public async Task<TEntity?> GetFirstByPredicateAsync(Expression<Func<TEntity, bool>> predicate)
      {
          return await _dbSet.FirstOrDefaultAsync(predicate);
      }
  }

public class EfCoreRepository<TEntity, TKey, TDbContext> : EfCoreRepository<TEntity, TDbContext>, IRepository<TEntity, TKey>
      where TEntity : class, IEntity where TDbContext : DbContext
  {
      public EfCoreRepository(TDbContext dbContext) : base(dbContext)
      {
      }

      public async Task<TEntity?> GetByIdAsync(TKey id)
      {
          return await _dbSet.FindAsync(id);
      }

      public async Task DeleteByIdAsync(TKey id)
      {
          var entity = await GetByIdAsync(id);
          if (entity != null)
          {
              _dbSet.Remove(entity);
              await _dbContext.SaveChangesAsync();
          }
      }

      public async Task<IEnumerable<TEntity>> GetByPredicateAsync(Expression<Func<TEntity, bool>> predicate)
      {
          return await _dbSet.Where(predicate).ToListAsync();
      }

    
  }
