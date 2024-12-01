using Bonyan.EntityFrameworkCore;
using Bonyan.Layer.Domain.Entities;
using Bonyan.Layer.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.Layer.Domain
{
    public class EfCoreBonRepository<TEntity, TDbContext> : EfCoreReadonlyRepository<TEntity, TDbContext>,
        IEfCoreBonRepository<TEntity>
        where TEntity : class, IBonEntity
        where TDbContext :  IEfDbContext
    {
   

        public async Task<TEntity> AddAsync(TEntity entity, bool autoSave = false)
        {
            BonEntityHelper.TrySetTenantId(entity, CurrentTenant?.Id);
            var dbContext = await GetDbContextAsync();

            var savedEntity = (await dbContext.Set<TEntity>().AddAsync(entity)).Entity;

            if (autoSave)
            {
                await dbContext.SaveChangesAsync();
            }

            return savedEntity;
        }

        public async Task UpdateAsync(TEntity entity, bool autoSave = false)
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

        public async Task DeleteAsync(TEntity entity, bool autoSave = false)
        {
            var dbContext = await GetDbContextAsync();

            dbContext.Set<TEntity>().Remove(entity);

            if (autoSave)
            {
                await dbContext.SaveChangesAsync();
            }
        }


        // Bulk Add
        public async Task AddRangeAsync(IEnumerable<TEntity> entities, bool autoSave = false)
        {
            var dbContext = await GetDbContextAsync();

            foreach (var entity in entities)
            {
                BonEntityHelper.TrySetTenantId(entity, CurrentTenant?.Id);
            }

            await dbContext.Set<TEntity>().AddRangeAsync(entities);

            if (autoSave)
            {
                await dbContext.SaveChangesAsync();
            }
        }

        // Bulk Update
        public async Task UpdateRangeAsync(IEnumerable<TEntity> entities, bool autoSave = false)
        {
            var dbContext = await GetDbContextAsync();

            foreach (var entity in entities)
            {
                if (dbContext.Set<TEntity>().Local.All(e => e != entity))
                {
                    dbContext.Set<TEntity>().Attach(entity);
                }

                dbContext.Entry(entity).State = EntityState.Modified;
            }

            if (autoSave)
            {
                await dbContext.SaveChangesAsync();
            }
        }

        // Bulk Delete by Entities
        public async Task DeleteRangeAsync(IEnumerable<TEntity> entities, bool autoSave = false)
        {
            var dbContext = await GetDbContextAsync();

            dbContext.Set<TEntity>().RemoveRange(entities);

            if (autoSave)
            {
                await dbContext.SaveChangesAsync();
            }
        }
    }

    public class EfCoreBonRepository<TEntity, TKey, TDbContext> : EfCoreReadonlyRepository<TEntity, TKey, TDbContext>,
        IEfCoreBonRepository<TEntity, TKey>
        where TEntity : class, IBonEntity<TKey>
        where TDbContext :  IEfDbContext
        where TKey : notnull
    {
    
        public async Task DeleteByIdAsync(TKey id, bool autoSave = false)
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

        public async Task<TEntity> AddAsync(TEntity entity, bool autoSave = false)
        {
            BonEntityHelper.TrySetTenantId(entity, CurrentTenant?.Id);
            var dbContext = await GetDbContextAsync();

            var savedEntity = (await dbContext.Set<TEntity>().AddAsync(entity)).Entity;

            if (autoSave)
            {
                await dbContext.SaveChangesAsync();
            }

            return savedEntity;
        }

        public async Task UpdateAsync(TEntity entity, bool autoSave = false)
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

        public async Task DeleteAsync(TEntity entity, bool autoSave = false)
        {
            var dbContext = await GetDbContextAsync();

            dbContext.Set<TEntity>().Remove(entity);

            if (autoSave)
            {
                await dbContext.SaveChangesAsync();
            }
        }

        // Bulk Add
        public async Task AddRangeAsync(IEnumerable<TEntity> entities, bool autoSave = false)
        {
            var dbContext = await GetDbContextAsync();

            foreach (var entity in entities)
            {
                BonEntityHelper.TrySetTenantId(entity, CurrentTenant?.Id);
            }

            await dbContext.Set<TEntity>().AddRangeAsync(entities);

            if (autoSave)
            {
                await dbContext.SaveChangesAsync();
            }
        }

        // Bulk Update
        public async Task UpdateRangeAsync(IEnumerable<TEntity> entities, bool autoSave = false)
        {
            var dbContext = await GetDbContextAsync();

            foreach (var entity in entities)
            {
                if (dbContext.Set<TEntity>().Local.All(e => e != entity))
                {
                    dbContext.Set<TEntity>().Attach(entity);
                }

                dbContext.Entry(entity).State = EntityState.Modified;
            }

            if (autoSave)
            {
                await dbContext.SaveChangesAsync();
            }
        }

        // Bulk Delete by Entities
        public async Task DeleteRangeAsync(IEnumerable<TEntity> entities, bool autoSave = false)
        {
            var dbContext = await GetDbContextAsync();

            dbContext.Set<TEntity>().RemoveRange(entities);

            if (autoSave)
            {
                await dbContext.SaveChangesAsync();
            }
        }
    }
}