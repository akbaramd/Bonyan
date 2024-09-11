using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Bonyan.DomainDrivenDesign.Domain.Abstractions;
using Bonyan.DomainDrivenDesign.Domain.Entities;
using Bonyan.DomainDrivenDesign.Domain.Specifications;

namespace Bonyan.DDD.Domain
{
    public class EfCoreRepository<TEntity, TDbContext> : IRepository<TEntity>
        where TEntity : class, IEntity where TDbContext : DbContext
    {
        protected readonly TDbContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;
        protected readonly ITenantAccessor _tenantAccessor;

        public EfCoreRepository(TDbContext dbContext, ITenantAccessor tenantAccessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _dbSet = _dbContext.Set<TEntity>();
            _tenantAccessor = tenantAccessor ;
        }

        public IQueryable<TEntity> ApplyTenantFilter(IQueryable<TEntity> query)
        {
            var currentTenant = _tenantAccessor?.CurrentTenant;

            // Apply tenant filtering only if the entity implements ITenant and a current tenant exists
            if (typeof(ITenant).IsAssignableFrom(typeof(TEntity)) && currentTenant != null)
            {
                query = query.Where(e => currentTenant.Contains(((ITenant)e).Tenant));
            }

            return query;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var query = ApplyTenantFilter(_dbSet.AsQueryable());
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var query = ApplyTenantFilter(_dbSet.AsQueryable().Where(predicate));
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Specification<TEntity> specification)
        {
            var query = ApplyTenantFilter(specification.Apply(_dbSet.AsQueryable()));
            return await query.ToListAsync();
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            // Check if the entity implements ITenant and verify tenant ownership
            if (entity is ITenant tenantEntity)
            {
                var currentTenant = _tenantAccessor?.CurrentTenant;
                if (currentTenant == null || !currentTenant.Contains(tenantEntity.Tenant))
                {
                    throw new UnauthorizedAccessException("Tenant mismatch. Cannot add entity.");
                }
            }

            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(TEntity entity)
        {
            // Check if the entity implements ITenant and verify tenant ownership
            if (entity is ITenant tenantEntity)
            {
                var currentTenant = _tenantAccessor?.CurrentTenant;
                if (currentTenant == null || !currentTenant.Contains(tenantEntity.Tenant))
                {
                    throw new UnauthorizedAccessException("Tenant mismatch. Cannot update entity.");
                }
            }

            _dbSet.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            // Check if the entity implements ITenant and verify tenant ownership
            if (entity is ITenant tenantEntity)
            {
                var currentTenant = _tenantAccessor?.CurrentTenant;
                if (currentTenant == null || !currentTenant.Contains(tenantEntity.Tenant))
                {
                    throw new UnauthorizedAccessException("Tenant mismatch. Cannot delete entity.");
                }
            }

            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var query = ApplyTenantFilter(_dbSet.AsQueryable().Where(predicate));
            return await query.CountAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var query = ApplyTenantFilter(_dbSet.AsQueryable().Where(predicate));
            return await query.AnyAsync();
        }

        public async Task<TEntity?> GetSingleByPredicateAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var query = ApplyTenantFilter(_dbSet.AsQueryable().Where(predicate));
            return await query.SingleOrDefaultAsync();
        }

        public async Task<TEntity?> GetFirstByPredicateAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var query = ApplyTenantFilter(_dbSet.AsQueryable().Where(predicate));
            return await query.FirstOrDefaultAsync();
        }
    }

    public class EfCoreRepository<TEntity, TKey, TDbContext> : EfCoreRepository<TEntity, TDbContext>, IRepository<TEntity, TKey>
        where TEntity : class, IEntity where TDbContext : DbContext
    {
        public EfCoreRepository(TDbContext dbContext, ITenantAccessor tenantAccessor) : base(dbContext, tenantAccessor)
        {
        }

        public async Task<TEntity?> GetByIdAsync(TKey id)
        {
            var entity = await _dbSet.FindAsync(id);

            // Check if the entity implements ITenant and verify tenant ownership
            if (entity is ITenant tenantEntity)
            {
                var currentTenant = _tenantAccessor?.CurrentTenant;
                if (currentTenant == null || !currentTenant.Contains(tenantEntity.Tenant))
                {
                    throw new UnauthorizedAccessException("Tenant mismatch. Cannot access entity.");
                }
            }

            return entity;
        }

        public async Task DeleteByIdAsync(TKey id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                // Check if the entity implements ITenant and verify tenant ownership
                if (entity is ITenant tenantEntity)
                {
                    var currentTenant = _tenantAccessor?.CurrentTenant;
                    if (currentTenant == null || !currentTenant.Contains(tenantEntity.Tenant))
                    {
                        throw new UnauthorizedAccessException("Tenant mismatch. Cannot delete entity.");
                    }
                }

                _dbSet.Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<TEntity>> GetByPredicateAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var query = ApplyTenantFilter(_dbSet.AsQueryable().Where(predicate));
            return await query.ToListAsync();
        }
    }
}
