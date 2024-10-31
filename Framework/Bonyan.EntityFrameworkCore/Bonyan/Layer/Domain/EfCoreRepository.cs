using Bonyan.EntityFrameworkCore;
using Bonyan.Layer.Domain.Entities;

namespace Bonyan.Layer.Domain
{
  public class EfCoreRepository<TEntity, TDbContext> : EfCoreReadonlyRepository<TEntity, TDbContext>,
    IEfCoreRepository<TEntity>
    where TEntity : class, IEntity
    where TDbContext : BonyanDbContext<TDbContext>
  {
    public EfCoreRepository(TDbContext dbContext)
      : base(dbContext)
    {
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
      EntityHelper.TrySetTenantId(entity, CurrentTenant?.Id);
      await (await GetDbContextAsync()).Set<TEntity>().AddAsync(entity);
      return entity;
    }

    public async Task UpdateAsync(TEntity entity)
    {
      EntityHelper.TrySetTenantId(entity, CurrentTenant?.Id);
       (await GetDbContextAsync()).Set<TEntity>().Update(entity);
    
    }

    public async Task DeleteAsync(TEntity entity)
    {
      EntityHelper.TrySetTenantId(entity, CurrentTenant?.Id);
      (await GetDbContextAsync()).Set<TEntity>().Remove(entity);
    }
  }

  public class EfCoreRepository<TEntity, TKey, TDbContext> : EfCoreReadonlyRepository<TEntity, TKey, TDbContext>,
    IEfCoreRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TDbContext : BonyanDbContext<TDbContext>
    where TKey : notnull
  {
    public EfCoreRepository(TDbContext userManagementDbContext)
      : base(userManagementDbContext)
    {
    }

    public async Task DeleteByIdAsync(TKey id)
    {
      var entity = await GetByIdAsync(id);
      if (entity == null) throw new KeyNotFoundException("Entity not found for deletion.");

      (await GetDbContextAsync()).Set<TEntity>().Remove(entity);
    
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
      EntityHelper.TrySetTenantId(entity, CurrentTenant?.Id);

      await (await GetDbContextAsync()).Set<TEntity>().AddAsync(entity);
      return entity;
    }

    public async Task UpdateAsync(TEntity entity)
    {
      var dbContext = await GetDbContextAsync();

      if (dbContext.Set<TEntity>().Local.All(e => e != entity))
      {
        dbContext.Set<TEntity>().Attach(entity);
        dbContext.Update(entity);

        await dbContext.SaveChangesAsync();
      }
    }

    public async Task DeleteAsync(TEntity entity)
    {
      EntityHelper.TrySetTenantId(entity, CurrentTenant?.Id);
      (await GetDbContextAsync()).Set<TEntity>().Remove(entity);
    }
  }
}
