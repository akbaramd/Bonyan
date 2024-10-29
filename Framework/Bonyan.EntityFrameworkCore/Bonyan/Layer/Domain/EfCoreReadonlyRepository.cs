using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Bonyan.DependencyInjection;
using Bonyan.IoC.Autofac;
using Bonyan.Layer.Domain.Entities;
using Bonyan.Layer.Domain.Model;
using Bonyan.Layer.Domain.Specifications;
using Bonyan.MultiTenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Layer.Domain
{
    public  class EfCoreReadonlyRepository<TEntity, TDbContext> : IReadonlyEfCoreRepository<TEntity>
        where TEntity : class, IEntity
        where TDbContext : DbContext
    {
        protected readonly TDbContext _dbContext;

        protected readonly DbSet<TEntity> _dbSet;
        
        public IBonyanLazyServiceProvider LazyServiceProvider { get; set; } = default!;
        
        
        public EfCoreReadonlyRepository(TDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _dbSet = _dbContext.Set<TEntity>();

        }

        public IQueryable<TEntity> GetQuery()
        {
            var query = _dbContext.Set<TEntity>().AsQueryable();

          

            return query;
        }

        protected bool IsMultiTenantEntity()
        {
          return CurrentTenant != null && typeof(IMultiTenant).IsAssignableFrom(typeof(TEntity)) && CurrentTenant.IsAvailable && CurrentTenant.Id.HasValue;
        }


        // Retrieve all entities of type T
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await GetQuery().ToListAsync();
        }

        // Find entities matching a specific predicate (filter)
        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetQuery().Where(predicate).ToListAsync();
        }

        // Find the first entity that matches a specific predicate (filter)
        public async Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetQuery().FirstOrDefaultAsync(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetQuery().Where(predicate).CountAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetQuery().Where(predicate).AnyAsync();
        }

        // Find entities based on a specification
        public async Task<IEnumerable<TEntity>> FindAsync(ISpecification<TEntity> specification)
        {
            if (specification is PaginatedSpecification<TEntity> s)
            {
                var query = ApplyPaginatedSpecification(GetQuery(), s);
                return await query.ToListAsync();
            }
            else
            {
                var query = ApplySpecification(GetQuery(), specification);
                return await query.ToListAsync();
            }
        }

        // Find the first entity that matches a specification
        public async Task<TEntity?> FindOneAsync(ISpecification<TEntity> specification)
        {
            var query = ApplySpecification(GetQuery(), specification);
            return await query.FirstOrDefaultAsync();
        }

  

        // Retrieve one entity matching a specific predicate, or throw an exception
        public async Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await FindOneAsync(predicate) ?? throw new Exception($"Entity with predicate {predicate} not found");
        }

        // Retrieve one entity matching a specification, or throw an exception
        public async Task<TEntity> GetOneAsync(ISpecification<TEntity> specification)
        {
            return await FindOneAsync(specification) ?? throw new Exception($"Entity with specification {specification} not found");
        }

        // Retrieve paginated results based on a PaginatedSpecification
        public async Task<PaginatedResult<TEntity>> PaginatedAsync(PaginatedSpecification<TEntity> paginateSpecification)
        {
            var query = ApplySpecification(GetQuery(), paginateSpecification);

            var totalCount = await query.CountAsync();
            var paginatedResults = await query.Skip(paginateSpecification.Skip)
                                              .Take(paginateSpecification.Take)
                                              .ToListAsync();

            return new PaginatedResult<TEntity>(paginatedResults, paginateSpecification.Skip, paginateSpecification.Take, totalCount);
        }
  
        public async Task<PaginatedResult<TEntity>> PaginatedAsync(PaginatedAndSortableSpecification<TEntity> paginateSpecification)
        {
            var query = ApplySpecification(GetQuery(), paginateSpecification);

            var totalCount = await query.CountAsync();

            if (!string.IsNullOrEmpty(paginateSpecification.SortBy))
            {
                var sortExpression = $"{paginateSpecification.SortBy} {paginateSpecification.SortDirection}";
                query = query.OrderBy(sortExpression);
            }

            var paginatedResults = await query.Skip(paginateSpecification.Skip)
                                              .Take(paginateSpecification.Take)
                                              .ToListAsync();

            return new PaginatedResult<TEntity>(paginatedResults, paginateSpecification.Skip, paginateSpecification.Take, totalCount);
        }

        public async Task<PaginatedResult<TEntity>> PaginatedAsync(Expression<Func<TEntity, bool>> predicate, int take, int skip)
        {
            var query = GetQuery().Where(predicate);
            var totalCount = await query.CountAsync();

            var results = await query.Skip(skip)
                                     .Take(take)
                                     .ToListAsync();

            return new PaginatedResult<TEntity>(results, skip, take, totalCount);
        }

        private IQueryable<TEntity> ApplySpecification(IQueryable<TEntity> query, ISpecification<TEntity> specification)
        {
            var ctx = new SpecificationContext<TEntity>(query);
            specification.Handle(ctx);
            return ctx.Query;
        }
  
        private IQueryable<TEntity> ApplyPaginatedSpecification(IQueryable<TEntity> query, PaginatedSpecification<TEntity> specification)
        {
            query = ApplySpecification(query, specification);
            query = query.Skip(specification.Skip).Take(specification.Take);
            return query;
        }

        public bool? IsChangeTrackingEnabled { get; }
        public ICurrentTenant? CurrentTenant => LazyServiceProvider.LazyGetService<ICurrentTenant>();
    }
    
    public class EfCoreReadonlyRepository<TEntity, TKey, TDbContext> : EfCoreReadonlyRepository<TEntity, TDbContext>, IReadonlyEfCoreRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
        where TDbContext : DbContext
        where TKey : notnull
    {
        public EfCoreReadonlyRepository(TDbContext dbContext) 
            : base(dbContext) { }

        public async Task<TEntity?> GetByIdAsync(TKey id)
        {
            return await GetQuery().FirstOrDefaultAsync(e => e.Id!.Equals(id));
        }
        // Retrieve entity by ID or throw an exception if not found
        // Find entity by its unique identifier
        public async Task<TEntity?> FindByIdAsync(TKey id)
        {
          return await GetQuery().FirstOrDefaultAsync(e => e.Id.Equals(id));
        }



        public async Task<IEnumerable<TEntity>> GetByPredicateAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetQuery().Where(predicate).ToListAsync();
        }
    }
}
