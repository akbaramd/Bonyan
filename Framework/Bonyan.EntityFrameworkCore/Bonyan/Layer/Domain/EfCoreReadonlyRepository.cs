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

        protected virtual IQueryable<TEntity> PrepareQuery(DbSet<TEntity> dbSet)
        {
            return dbSet;
        }

        internal async Task<IQueryable<TEntity>> GetPreparedQueryAsync()
        {
            var dbContext = await GetDbContextAsync();
            var dbSet = dbContext.Set<TEntity>();
            return PrepareQuery(dbSet);
        }

        public async Task<TDbContext> GetDbContextAsync()
        {
            return await DbContextProvider.GetDbContextAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var query = await GetPreparedQueryAsync();
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var query = await GetPreparedQueryAsync();
            return await query.Where(predicate).ToListAsync();
        }

        public async Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var query = await GetPreparedQueryAsync();
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var query = await GetPreparedQueryAsync();
            return await query.CountAsync(predicate);
        }

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var query = await GetPreparedQueryAsync();
            return await query.AnyAsync(predicate);
        }

        public async Task<IEnumerable<TEntity>> FindAsync(ISpecification<TEntity> specification)
        {
            var query = await GetPreparedQueryAsync();
            query = specification is PaginatedSpecification<TEntity> paginatedSpec
                ? ApplyPaginatedSpecification(query, paginatedSpec)
                : ApplySpecification(query, specification);
            return await query.ToListAsync();
        }

        public async Task<TEntity?> FindOneAsync(ISpecification<TEntity> specification)
        {
            var query = ApplySpecification(await GetPreparedQueryAsync(), specification);
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
            var query = ApplySpecification(await GetPreparedQueryAsync(), paginateSpecification);
            var totalCount = await query.CountAsync();
            var results = await query.Skip(paginateSpecification.Skip).Take(paginateSpecification.Take).ToListAsync();

            return new PaginatedResult<TEntity>(results, paginateSpecification.Skip, paginateSpecification.Take, totalCount);
        }

        public async Task<PaginatedResult<TEntity>> PaginatedAsync(PaginatedAndSortableSpecification<TEntity> paginateSpecification)
        {
            var query = ApplySpecification(await GetPreparedQueryAsync(), paginateSpecification);
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
            var query = (await GetPreparedQueryAsync()).Where(predicate);
            var totalCount = await query.CountAsync();
            var results = await query.Skip(skip).Take(take).ToListAsync();

            return new PaginatedResult<TEntity>(results, skip, take, totalCount);
        }

        private static IQueryable<TEntity> ApplySpecification(IQueryable<TEntity> query, ISpecification<TEntity> specification)
        {
            var ctx = new SpecificationContext<TEntity>(query);
            specification.Handle(ctx);
            return ctx.Query;
        }

        private static IQueryable<TEntity> ApplyPaginatedSpecification(IQueryable<TEntity> query, PaginatedSpecification<TEntity> specification)
        {
            return ApplySpecification(query, specification).Skip(specification.Skip).Take(specification.Take);
        }

        public bool? IsChangeTrackingEnabled { get; }
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
            return await (await GetPreparedQueryAsync()).FirstOrDefaultAsync(e => e.Id!.Equals(id));
        }

        public async Task<TEntity?> FindByIdAsync(TKey id)
        {
            return await (await GetPreparedQueryAsync()).FirstOrDefaultAsync(e => e.Id.Equals(id));
        }

        public async Task<IEnumerable<TEntity>> GetByPredicateAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await (await GetPreparedQueryAsync()).Where(predicate).ToListAsync();
        }
    }
}
