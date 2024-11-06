using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Bonyan.DependencyInjection;
using Bonyan.EntityFrameworkCore;
using Bonyan.EntityFrameworkCore.Abstractions;
using Bonyan.Exceptions;
using Bonyan.Layer.Domain.Entities;
using Bonyan.Layer.Domain.Model;
using Bonyan.Layer.Domain.Specifications;
using Bonyan.MultiTenant;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.Layer.Domain
{
    public abstract class EfCoreReadonlyRepository<TEntity, TDbContext> : IReadonlyEfCoreRepository<TEntity>
        where TEntity : class, IEntity
        where TDbContext : DbContext, IBonyanDbContext<TDbContext>
    {
        protected EfCoreReadonlyRepository(TDbContext userManagementDbContext)
        {
        }

        public IBonyanLazyServiceProvider LazyServiceProvider { get; set; } = default!;
        public IDbContextProvider<TDbContext> DbContextProvider { get; set; } = default!;
        public ICurrentTenant? CurrentTenant => LazyServiceProvider.LazyGetService<ICurrentTenant>();

        // Virtual method to allow including related entities or applying custom logic in derived repositories
        protected virtual IQueryable<TEntity> PrepareQuery(DbSet<TEntity> dbSet) => dbSet;

        // Retrieves DbContext instance
        internal async Task<IQueryable<TEntity>> GetQueryAsync()
        {
            var dbContext = await GetDbContextAsync();
            var query = PrepareQuery(dbContext.Set<TEntity>());
            
            // Apply AsNoTracking() if change tracking is disabled
            if (IsChangeTrackingEnabled.HasValue && IsChangeTrackingEnabled.Value)
            {
                query = query.AsNoTracking();
            }

            return query;
        }

        public async Task<TDbContext> GetDbContextAsync() => await DbContextProvider.GetDbContextAsync();


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

        public async Task<IEnumerable<TEntity>> FindAsync(ISpecification<TEntity> specification)
        {
            var query = await GetQueryAsync();
            query = ApplySpecification(query, specification);
            return await query.ToListAsync();
        }

        public async Task<TEntity?> FindOneAsync(ISpecification<TEntity> specification)
        {
            var query = ApplySpecification(await GetQueryAsync(), specification);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await FindOneAsync(predicate) 
                ?? throw new BonyanException($"Entity with predicate {predicate} not found");
        }

        public async Task<TEntity> GetOneAsync(ISpecification<TEntity> specification)
        {
            return await FindOneAsync(specification) 
                ?? throw new BonyanException($"Entity with specification {specification} not found");
        }

        public async Task<PaginatedResult<TEntity>> PaginatedAsync(PaginatedSpecification<TEntity> paginateSpecification)
        {
            var query = ApplySpecification(await GetQueryAsync(), paginateSpecification);
            var totalCount = await query.CountAsync();
            var results = await query.Skip(paginateSpecification.Skip).Take(paginateSpecification.Take).ToListAsync();

            return new PaginatedResult<TEntity>(results, paginateSpecification.Skip, paginateSpecification.Take, totalCount);
        }

        public async Task<PaginatedResult<TEntity>> PaginatedAsync(PaginatedAndSortableSpecification<TEntity> paginateSpecification)
        {
            var query = ApplySpecification(await GetQueryAsync(), paginateSpecification);
            var totalCount = await query.CountAsync();

            if (!string.IsNullOrEmpty(paginateSpecification.SortBy))
            {
                query = query.OrderBy($"{paginateSpecification.SortBy} {paginateSpecification.SortDirection}");
            }

            var results = await query.Skip(paginateSpecification.Skip).Take(paginateSpecification.Take).ToListAsync();

            return new PaginatedResult<TEntity>(results, paginateSpecification.Skip, paginateSpecification.Take, totalCount);
        }

        public async Task<PaginatedResult<TEntity>> PaginatedAsync(Expression<Func<TEntity, bool>> predicate, int take, int skip)
        {
            var query = (await GetQueryAsync()).Where(predicate);
            var totalCount = await query.CountAsync();
            var results = await query.Skip(skip).Take(take).ToListAsync();

            return new PaginatedResult<TEntity>(results, skip, take, totalCount);
        }

        private static IQueryable<TEntity> ApplySpecification(IQueryable<TEntity> query, ISpecification<TEntity> specification)
        {
            var context = new SpecificationContext<TEntity>(query);
            specification.Handle(context);
            return context.Query;
        }

        private static IQueryable<TEntity> ApplyPagination(IQueryable<TEntity> query, PaginatedSpecification<TEntity> specification)
        {
            return ApplySpecification(query, specification).Skip(specification.Skip).Take(specification.Take);
        }

        public bool? IsChangeTrackingEnabled { get; } = true;
        public IQueryable<TEntity> Queryable => GetDbContextAsync().GetAwaiter().GetResult().Set<TEntity>();
    }

    public class EfCoreReadonlyRepository<TEntity, TKey, TDbContext> : EfCoreReadonlyRepository<TEntity, TDbContext>, IReadonlyEfCoreRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
        where TDbContext : DbContext, IBonyanDbContext<TDbContext>
        where TKey : notnull
    {
        public EfCoreReadonlyRepository(TDbContext userManagementDbContext) 
            : base(userManagementDbContext) { }

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
