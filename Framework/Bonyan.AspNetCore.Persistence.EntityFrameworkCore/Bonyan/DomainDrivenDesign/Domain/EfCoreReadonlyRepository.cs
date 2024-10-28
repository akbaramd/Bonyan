using System.Linq.Expressions;
using Bonyan.DomainDrivenDesign.Domain.Abstractions;
using Bonyan.DomainDrivenDesign.Domain.Entities;
using Bonyan.DomainDrivenDesign.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Bonyan.DomainDrivenDesign.Domain.Model;

namespace Bonyan.DomainDrivenDesign.Domain
{
    public class EfCoreReadonlyRepository<TEntity, TDbContext> : IReadonlyEfCoreRepository<TEntity>
        where TEntity : class, IEntity
        where TDbContext : DbContext
    {
        protected readonly TDbContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;

        public EfCoreReadonlyRepository(TDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _dbSet = _dbContext.Set<TEntity>();
        }

        // Find entity by its unique identifier
        public async Task<TEntity?> FindByIdAsync(Guid id)
        {
          return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        // Retrieve all entities of type T
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
          return await _dbContext.Set<TEntity>().ToListAsync();
        }

        // Find entities matching a specific predicate (filter)
        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
          return await _dbContext.Set<TEntity>().Where(predicate).ToListAsync();
        }

        // Find the first entity that matches a specific predicate (filter)
        public async Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> predicate)
        {
          return await _dbContext.Set<TEntity>().FirstOrDefaultAsync(predicate);
        }
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.AsQueryable().Where(predicate).CountAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.AsQueryable().Where(predicate).AnyAsync();
        }
 // Find entities based on a specification
  public async Task<IEnumerable<TEntity>> FindAsync(ISpecification<TEntity> specification)
  {
    if (specification is PaginatedSpecification<TEntity> s)
    {
      var res=_dbContext.Set<TEntity>();
      var query = ApplyPaginatedSpecification(res, s);
      return await query.ToListAsync();
    }
    else
    {
      var query = ApplySpecification(_dbContext.Set<TEntity>().AsQueryable(), specification);
      return await query.ToListAsync();
    }
  }

  // Find the first entity that matches a specification
  public async Task<TEntity?> FindOneAsync(ISpecification<TEntity> specification)
  {
    var query = ApplySpecification(_dbContext.Set<TEntity>().AsQueryable(), specification);
    return await query.FirstOrDefaultAsync();
  }

  // Retrieve entity by ID or throw an exception if not found
  public async Task<TEntity> GetByIdAsync(Guid id)
  {
    return await _dbContext.Set<TEntity>().FindAsync(id) ?? throw new Exception($"Entity with id {id} not found");
  }

  // Retrieve one entity matching a specific predicate, or throw an exception
  public async Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> predicate)
  {
    return await FindOneAsync(predicate) ?? throw new Exception($"Entity with predicate {predicate} not found");
  }

  // Retrieve one entity matching a specification, or throw an exception
  public async Task<TEntity> GetOneAsync(ISpecification<TEntity> specification)
  {
    return await FindOneAsync(specification) ??
           throw new Exception($"Entity with specification {specification} not found");
  }

  // Retrieve paginated results based on a PaginatedSpecification
  public async Task<PaginatedResult<TEntity>> PaginatedAsync(PaginatedSpecification<TEntity> paginateSpecification)
  {
    // Initialize query without making multiple calls to _dbContext.Set<TEntity>()
    var query = _dbContext.Set<TEntity>().AsQueryable();

    // Apply the specification (criteria, includes, etc.)
    query = ApplySpecification(query, paginateSpecification);

    // Capture the total count before applying pagination
    var totalCount = await query.CountAsync();

    // Apply pagination logic (Skip and Take)
    var paginatedResults = await query.Skip(paginateSpecification.Skip)
      .Take(paginateSpecification.Take)
      .ToListAsync();

    return new PaginatedResult<TEntity>(paginatedResults, paginateSpecification.Skip, paginateSpecification.Take, totalCount);
  }
  
  public async Task<PaginatedResult<TEntity>> PaginatedAsync(PaginatedAndSortableSpecification<TEntity> paginateSpecification)
  {
    // Initialize query
    var query = _dbContext.Set<TEntity>().AsQueryable();

    // Apply the specification (criteria, includes, etc.)
    query = ApplySpecification(query, paginateSpecification);

    // Capture total count before pagination
    var totalCount = await query.CountAsync();

    // Apply dynamic sorting using string property names and directions
    if (!string.IsNullOrEmpty(paginateSpecification.SortBy))
    {
      var sortExpression = $"{paginateSpecification.SortBy} {paginateSpecification.SortDirection}";
      query = query.OrderBy(sortExpression); // Using System.Linq.Dynamic.Core's OrderBy
    }

    // Apply pagination (Skip and Take)
    var paginatedResults = await query.Skip(paginateSpecification.Skip)
      .Take(paginateSpecification.Take)
      .ToListAsync();

    return new PaginatedResult<TEntity>(paginatedResults, paginateSpecification.Skip, paginateSpecification.Take, totalCount);
  }

  
  // Retrieve paginated results based on a predicate, skip, and take values
  public async Task<PaginatedResult<TEntity>> PaginatedAsync(Expression<Func<TEntity, bool>> predicate, int take, int skip)
  {
    var totalCount = await _dbContext.Set<TEntity>().Where(predicate).CountAsync();

    var results = await _dbContext.Set<TEntity>()
      .Where(predicate)
      .Skip(skip)
      .Take(take)
      .ToListAsync();

    return new PaginatedResult<TEntity>(results, skip, take, totalCount);
  }

  // Helper method to apply specifications (criteria, includes, ordering, pagination) to an IQueryable
  private IQueryable<TEntity> ApplySpecification(IQueryable<TEntity> query, ISpecification<TEntity> specification)
  {
    var ctx = new SpecificationContext<TEntity>(query);
    specification.Handle(ctx);
    return ctx.Query; // Use the modified query from context
  }
  
// Helper method to apply paginated specifications (criteria, includes, ordering, pagination)
  private IQueryable<TEntity> ApplyPaginatedSpecification(IQueryable<TEntity> query, PaginatedSpecification<TEntity> specification)
  {
    // Reuse ApplySpecification to avoid duplicating the same logic
    query = ApplySpecification(query, specification);

    // Apply pagination (Skip and Take)
    query = query.Skip(specification.Skip).Take(specification.Take);
    return query;
  }
        public bool? IsChangeTrackingEnabled { get; }
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
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetByPredicateAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.AsQueryable().Where(predicate).ToListAsync();
        }
    }
}
