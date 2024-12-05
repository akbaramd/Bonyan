using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Bonyan.DependencyInjection;
using Bonyan.EntityFrameworkCore;
using Bonyan.Exceptions;
using Bonyan.Layer.Domain.Entity;
using Bonyan.Layer.Domain.Repository.Abstractions;
using Bonyan.Layer.Domain.Specification.Abstractions;
using Bonyan.Layer.Domain.Specifications;
using Bonyan.MultiTenant;
using Bonyan.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.Layer.Domain
{
    public abstract class EfCoreReadonlyRepository<TEntity, TDbContext> : IReadonlyEfCoreRepository<TEntity>,IBonUnitOfWorkEnabled
        where TEntity : class, IBonEntity
        where TDbContext : IEfDbContext
    {
  

        public IBonLazyServiceProvider LazyServiceProvider { get; set; } = default!;
        public IBonDbContextProvider<TDbContext> BonDbContextProvider => LazyServiceProvider.LazyGetRequiredService<IBonDbContextProvider<TDbContext>>();
        public IBonCurrentTenant? CurrentTenant => LazyServiceProvider.LazyGetService<IBonCurrentTenant>();

        // Virtual method to allow including related entities or applying custom logic in derived repositories
        protected virtual IQueryable<TEntity> PrepareQuery(DbSet<TEntity> dbSet) => dbSet;

        // Retrieves BonDbContext instance
        internal async Task<IQueryable<TEntity>> GetQueryAsync()
        {
            var dbContext = await GetDbContextAsync();
            var query = PrepareQuery(dbContext.Set<TEntity>());
            
            // Apply AsNoTracking() if change tracking is disabled
            if (IsChangeTrackingEnabled.HasValue && !IsChangeTrackingEnabled.Value)
            {
                query = query.AsNoTracking();
            }

            return query;
        }

        public async Task<TDbContext> GetDbContextAsync() => await BonDbContextProvider.GetDbContextAsync();


    

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var query = await GetQueryAsync();
            return await query.Where(predicate).ToListAsync();
        }

        public async Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var query = await GetQueryAsync();
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var query = await GetQueryAsync();
            return await query.CountAsync(predicate);
        }

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var query = await GetQueryAsync();
            return await query.AnyAsync(predicate);
        }

        public async Task<IEnumerable<TEntity>> FindAsync(IBonSpecification<TEntity> bonSpecification)
        {
            var query = await GetQueryAsync();
            query = ApplySpecification(query, bonSpecification);
            return await query.ToListAsync();
        }

        public async Task<TEntity?> FindOneAsync(IBonSpecification<TEntity> bonSpecification)
        {
            var query = ApplySpecification(await GetQueryAsync(), bonSpecification);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await FindOneAsync(predicate) 
                ?? throw new BonException($"Entity with predicate {predicate} not found");
        }

        public async Task<TEntity> GetOneAsync(IBonSpecification<TEntity> bonSpecification)
        {
            return await FindOneAsync(bonSpecification) 
                ?? throw new BonException($"Entity with specification {bonSpecification} not found");
        }

        public async Task<BonPaginatedResult<TEntity>> PaginatedAsync(BonPaginatedSpecification<TEntity> paginateSpecification)
        {
            var query = ApplySpecification(await GetQueryAsync(), paginateSpecification);
            var totalCount = await query.CountAsync();
            var results = await query.Skip(paginateSpecification.Skip).Take(paginateSpecification.Take).ToListAsync();

            return new BonPaginatedResult<TEntity>(results, paginateSpecification.Skip, paginateSpecification.Take, totalCount);
        }

        public async Task<BonPaginatedResult<TEntity>> PaginatedAsync(BonPaginatedAndSortableSpecification<TEntity> paginateSpecification)
        {
            var query = ApplySpecification(await GetQueryAsync(), paginateSpecification);
            var totalCount = await query.CountAsync();

            if (!string.IsNullOrEmpty(paginateSpecification.SortBy))
            {
                query = query.OrderBy($"{paginateSpecification.SortBy} {paginateSpecification.SortDirection}");
            }

            var results = await query.Skip(paginateSpecification.Skip).Take(paginateSpecification.Take).ToListAsync();

            return new BonPaginatedResult<TEntity>(results, paginateSpecification.Skip, paginateSpecification.Take, totalCount);
        }

        public async Task<BonPaginatedResult<TEntity>> PaginatedAsync(Expression<Func<TEntity, bool>> predicate, int take, int skip)
        {
            var query = (await GetQueryAsync()).Where(predicate);
            var totalCount = await query.CountAsync();
            var results = await query.Skip(skip).Take(take).ToListAsync();

            return new BonPaginatedResult<TEntity>(results, skip, take, totalCount);
        }

        private static IQueryable<TEntity> ApplySpecification(IQueryable<TEntity> query, IBonSpecification<TEntity> bonSpecification)
        {
            var context = new BonSpecificationContext<TEntity>(query);
            bonSpecification.Handle(context);
            return context.Query;
        }

        private static IQueryable<TEntity> ApplyPagination(IQueryable<TEntity> query, BonPaginatedSpecification<TEntity> specification)
        {
            return ApplySpecification(query, specification).Skip(specification.Skip).Take(specification.Take);
        }

        public bool? IsChangeTrackingEnabled { get; } = true;

        public async Task<IQueryable<TEntity>> GetQueryableAsync()
        {
            return (await GetDbContextAsync()).Set<TEntity>();
        }
    }

    public class EfCoreReadonlyRepository<TEntity, TKey, TDbContext> : EfCoreReadonlyRepository<TEntity, TDbContext>, IReadonlyEfCoreRepository<TEntity, TKey>
        where TEntity : class, IBonEntity<TKey>
        where TDbContext :IEfDbContext
        where TKey : notnull
    {
    

        public async Task<TEntity?> GetByIdAsync(TKey id)
        {
            return await (await GetQueryAsync()).FirstOrDefaultAsync(e => e.Id!.Equals(id));
        }

        public async Task<TEntity?> FindByIdAsync(TKey id)
        {
            return await (await GetQueryAsync()).FirstOrDefaultAsync(e => e.Id.Equals(id));
        }

        public async Task<IEnumerable<TEntity>> GetByPredicateAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await (await GetQueryAsync()).Where(predicate).ToListAsync();
        }
    }
}
