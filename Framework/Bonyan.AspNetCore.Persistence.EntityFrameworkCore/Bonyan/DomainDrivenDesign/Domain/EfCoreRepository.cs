using Bonyan.AspNetCore.Persistence.EntityFrameworkCore;
using Bonyan.DomainDrivenDesign.Domain.Abstractions;
using Bonyan.DomainDrivenDesign.Domain.Entities;
using Bonyan.MultiTenant;

namespace Bonyan.DomainDrivenDesign.Domain
{
  public class EfCoreRepository<TEntity, TDbContext> : EfCoreReadonlyRepository<TEntity, TDbContext>,
    IEfCoreRepository<TEntity>
    where TEntity : class, IEntity
    where TDbContext : BonyanDbContext<TDbContext>
  {
    public EfCoreRepository(TDbContext dbContext, IServiceProvider serviceProvider)
      : base(dbContext, serviceProvider)
    {
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
      EntityHelper.TrySetTenantId(entity, _currentTenant.Id);
      await _dbSet.AddAsync(entity);
      await _dbContext.SaveChangesAsync();
      return entity;
    }

    public async Task UpdateAsync(TEntity entity)
    {
      EntityHelper.TrySetTenantId(entity, _currentTenant.Id);
      _dbSet.Update(entity);
      await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(TEntity entity)
    {
      EntityHelper.TrySetTenantId(entity, _currentTenant.Id);
      _dbSet.Remove(entity);
      await _dbContext.SaveChangesAsync();
    }
  }

  public class EfCoreRepository<TEntity, TKey, TDbContext> : EfCoreReadonlyRepository<TEntity, TKey, TDbContext>,
    IEfCoreRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TDbContext : BonyanDbContext<TDbContext>
    where TKey : notnull
  {
    public EfCoreRepository(TDbContext dbContext, IServiceProvider serviceProvider)
      : base(dbContext, serviceProvider)
    {
    }

    public async Task DeleteByIdAsync(TKey id)
    {
      var entity = await GetByIdAsync(id);
      if (entity == null) throw new KeyNotFoundException("Entity not found for deletion.");

      _dbSet.Remove(entity);
      await _dbContext.SaveChangesAsync();
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
      EntityHelper.TrySetTenantId(entity, _currentTenant.Id);

      await _dbSet.AddAsync(entity);
      await _dbContext.SaveChangesAsync();
      return entity;
    }

    public async Task UpdateAsync(TEntity entity)
    {
      EntityHelper.TrySetTenantId(entity, _currentTenant.Id);
      _dbSet.Update(entity);
      await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(TEntity entity)
    {
      EntityHelper.TrySetTenantId(entity, _currentTenant.Id);
      _dbSet.Remove(entity);
      await _dbContext.SaveChangesAsync();
    }
  }
}
