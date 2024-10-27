using System.Linq.Expressions;
using Bonyan.DomainDrivenDesign.Domain.Abstractions;
using Bonyan.DomainDrivenDesign.Domain.Entities;
using Bonyan.DomainDrivenDesign.Domain.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.DomainDrivenDesign.Domain
{
    public class EfCoreReadonlyRepository<TEntity, TDbContext> : IReadonlyEfCoreRepository<TEntity>
        where TEntity : class, IEntity
        where TDbContext : DbContext
    {
        protected readonly TDbContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;
        protected readonly ITenantAccessor? _tenantAccessor;

        public EfCoreReadonlyRepository(TDbContext dbContext, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _dbSet = _dbContext.Set<TEntity>();
            _tenantAccessor = serviceProvider.GetService<ITenantAccessor>();
        }

        protected IQueryable<TEntity> ApplyTenantFilter(IQueryable<TEntity> query)
        {
            var currentTenant = _tenantAccessor?.CurrentTenant;
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

        public Task<IEnumerable<TEntity>> FindAsync(Specification<TEntity> specification)
        {
          //todo: implement this
          throw new NotImplementedException();
        }

        public async Task<TEntity?> GetFirstByPredicateAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var query = ApplyTenantFilter(_dbSet.AsQueryable().Where(predicate));
            return await query.FirstOrDefaultAsync();
        }

        public async Task<TEntity?> GetSingleByPredicateAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var query = ApplyTenantFilter(_dbSet.AsQueryable().Where(predicate));
            return await query.SingleOrDefaultAsync();
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

        public bool? IsChangeTrackingEnabled { get; }
        
        
    }
    
    
    public class EfCoreReadonlyRepository<TEntity, TKey, TDbContext> : EfCoreReadonlyRepository<TEntity, TDbContext>, IReadonlyEfCoreRepository<TEntity, TKey>
      where TEntity : class, IEntity<TKey>
      where TDbContext : DbContext
      where TKey : notnull
    {
      public EfCoreReadonlyRepository(TDbContext dbContext, IServiceProvider serviceProvider) 
        : base(dbContext, serviceProvider) { }

      public async Task<TEntity?> GetByIdAsync(TKey id)
      {
        var entity = await _dbSet.FindAsync(id);
        if (entity is ITenant tenantEntity && _tenantAccessor?.CurrentTenant is { } currentTenant && !currentTenant.Contains(tenantEntity.Tenant))
        {
          throw new UnauthorizedAccessException("Tenant mismatch. Cannot access entity.");
        }
        return entity;
      }

      public async Task<IEnumerable<TEntity>> GetByPredicateAsync(Expression<Func<TEntity, bool>> predicate)
      {
        var query = ApplyTenantFilter(_dbSet.AsQueryable().Where(predicate));
        return await query.ToListAsync();
      }
    }
}
