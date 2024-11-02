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

        public async Task<TEntity> AddAsync(TEntity entity,bool autoSave=  false)
        {
            EntityHelper.TrySetTenantId(entity, CurrentTenant?.Id);
            var dbContext = await GetDbContextAsync();

            var savedEntity = (await dbContext.Set<TEntity>().AddAsync(entity)).Entity;

            if (autoSave)
            {
                await dbContext.SaveChangesAsync();
            }
            return savedEntity;
        }

        public async Task UpdateAsync(TEntity entity,bool autoSave=  false)
        {
            var dbContext = await GetDbContextAsync();

            if (dbContext.Set<TEntity>().Local.All(e => e != entity))
            {
                dbContext.Set<TEntity>().Attach(entity);
                dbContext.Update(entity);
            }
            if (autoSave)
            {
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(TEntity entity,bool autoSave=  false)
        {
            var dbContext = await GetDbContextAsync();

            dbContext.Set<TEntity>().Remove(entity);
            
            if (autoSave)
            {
                await dbContext.SaveChangesAsync();
            }
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

        public async Task DeleteByIdAsync(TKey id,bool autoSave=  false)
        {
            var dbContext = await GetDbContextAsync();

            var entity = await GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException("Entity not found for deletion.");

            dbContext.Set<TEntity>().Remove(entity);
            
            if (autoSave)
            {
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<TEntity> AddAsync(TEntity entity,bool autoSave=  false)
        {
            EntityHelper.TrySetTenantId(entity, CurrentTenant?.Id);
            var dbContext = await GetDbContextAsync();

            var savedEntity = (await dbContext.Set<TEntity>().AddAsync(entity)).Entity;

            if (autoSave)
            {
                await dbContext.SaveChangesAsync();
            }
            return savedEntity;
        }

        public async Task UpdateAsync(TEntity entity,bool autoSave=  false)
        {
            var dbContext = await GetDbContextAsync();

            if (dbContext.Set<TEntity>().Local.All(e => e != entity))
            {
                dbContext.Set<TEntity>().Attach(entity);
                dbContext.Update(entity);
            }
            if (autoSave)
            {
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(TEntity entity,bool autoSave=  false)
        {
            var dbContext = await GetDbContextAsync();

            dbContext.Set<TEntity>().Remove(entity);
            
            if (autoSave)
            {
                await dbContext.SaveChangesAsync();
            }
        }
    }
}