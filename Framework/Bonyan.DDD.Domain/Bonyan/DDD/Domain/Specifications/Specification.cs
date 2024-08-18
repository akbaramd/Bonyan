using System.Linq.Expressions;
using Bonyan.DDD.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.DDD.Domain.Specifications;

/// <summary>
/// Base class for implementing specifications in a DDD context.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public abstract class Specification<TEntity> where TEntity : class, IEntity
{
    /// <summary>
    /// List of include expressions for eager loading.
    /// </summary>
    public List<Expression<Func<TEntity, object>>> Includes { get; } = new();

    /// <summary>
    /// Sorting criteria in ascending order.
    /// </summary>
    public Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderBy { get; private set; }

    /// <summary>
    /// Sorting criteria in descending order.
    /// </summary>
    public Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderByDescending { get; private set; }

    /// <summary>
    /// Number of records to take.
    /// </summary>
    public int? Take { get; private set; }

    /// <summary>
    /// Number of records to skip.
    /// </summary>
    public int? Skip { get; private set; }

    /// <summary>
    /// Criteria expression for the specification.
    /// </summary>
    public abstract Expression<Func<TEntity, bool>> ToExpression();

    /// <summary>
    /// Combines two specifications using logical AND.
    /// </summary>
    /// <param name="specification">The specification to combine with.</param>
    /// <returns>A new specification representing the AND combination.</returns>
    public Specification<TEntity> And(Specification<TEntity> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return new AndSpecification<TEntity>(this, specification);
    }

    /// <summary>
    /// Combines two specifications using logical OR.
    /// </summary>
    /// <param name="specification">The specification to combine with.</param>
    /// <returns>A new specification representing the OR combination.</returns>
    public Specification<TEntity> Or(Specification<TEntity> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return new OrSpecification<TEntity>(this, specification);
    }

    /// <summary>
    /// Negates the specification.
    /// </summary>
    /// <returns>A new specification representing the negation.</returns>
    public Specification<TEntity> Not()
    {
        return new NotSpecification<TEntity>(this);
    }

    /// <summary>
    /// Adds an include expression for eager loading.
    /// </summary>
    /// <param name="includeExpression">The include expression.</param>
    /// <returns>A new specification with the include expression added.</returns>
    public Specification<TEntity> AddInclude(Expression<Func<TEntity, object>> includeExpression)
    {
        ArgumentNullException.ThrowIfNull(includeExpression);
        var clone = Clone();
        clone.Includes.Add(includeExpression);
        return clone;
    }

    /// <summary>
    /// Applies sorting by criteria in ascending order.
    /// </summary>
    /// <param name="orderByExpression">The sorting expression.</param>
    /// <returns>A new specification with the sorting applied.</returns>
    public Specification<TEntity> ApplyOrderBy(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderByExpression)
    {
        ArgumentNullException.ThrowIfNull(orderByExpression);
        var clone = Clone();
        clone.OrderBy = orderByExpression;
        return clone;
    }

    /// <summary>
    /// Applies sorting by criteria in descending order.
    /// </summary>
    /// <param name="orderByDescendingExpression">The sorting expression.</param>
    /// <returns>A new specification with the sorting applied.</returns>
    public Specification<TEntity> ApplyOrderByDescending(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderByDescendingExpression)
    {
        ArgumentNullException.ThrowIfNull(orderByDescendingExpression);
        var clone = Clone();
        clone.OrderByDescending = orderByDescendingExpression;
        return clone;
    }

    /// <summary>
    /// Applies paging to the specification.
    /// </summary>
    /// <param name="skip">The number of records to skip.</param>
    /// <param name="take">The number of records to take.</param>
    /// <returns>A new specification with the paging applied.</returns>
    public Specification<TEntity> ApplyPaging(int skip, int take)
    {
        var clone = Clone();
        clone.Skip = skip;
        clone.Take = take;
        return clone;
    }

    /// <summary>
    /// Determines if an entity satisfies the specification.
    /// </summary>
    /// <param name="entity">The entity to test.</param>
    /// <returns>True if the entity satisfies the specification; otherwise, false.</returns>
    public bool IsSatisfiedBy(TEntity entity)
    {
        var predicate = ToExpression().Compile();
        return predicate(entity);
    }

    /// <summary>
    /// Applies the specification to a queryable source.
    /// </summary>
    /// <param name="query">The queryable source.</param>
    /// <returns>A queryable with the specification applied.</returns>
    public IQueryable<TEntity> Apply(IQueryable<TEntity> query)
    {
        query = query.Where(ToExpression());

        if (Includes.Any())
        {
            query = Includes.Aggregate(query, (current, include) => current.IncludeIfAvailable(include));
        }

        if (OrderBy != null)
        {
            query = OrderBy(query);
        }
        else if (OrderByDescending != null)
        {
            query = OrderByDescending(query);
        }

        if (Skip.HasValue)
        {
            query = query.Skip(Skip.Value);
        }

        if (Take.HasValue)
        {
            query = query.Take(Take.Value);
        }

        return query;
    }

    /// <summary>
    /// Creates a shallow copy of the specification.
    /// </summary>
    /// <returns>A new instance of the specification with the same properties.</returns>
    protected virtual Specification<TEntity> Clone()
    {
        return (Specification<TEntity>)MemberwiseClone();
    }
}

/// <summary>
/// Combines two specifications using logical AND.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public class AndSpecification<TEntity> : Specification<TEntity> where TEntity : class, IEntity
{
    private readonly Specification<TEntity> _left;
    private readonly Specification<TEntity> _right;

    public AndSpecification(Specification<TEntity> left, Specification<TEntity> right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        _left = left;
        _right = right;
    }

    public override Expression<Func<TEntity, bool>> ToExpression()
    {
        var leftExpression = _left.ToExpression();
        var rightExpression = _right.ToExpression();

        var parameter = Expression.Parameter(typeof(TEntity));
        var combinedExpression = Expression.Lambda<Func<TEntity, bool>>(
            Expression.AndAlso(
                Expression.Invoke(leftExpression, parameter),
                Expression.Invoke(rightExpression, parameter)
            ),
            parameter
        );

        return combinedExpression;
    }
}

/// <summary>
/// Combines two specifications using logical OR.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public class OrSpecification<TEntity> : Specification<TEntity> where TEntity : class, IEntity
{
    private readonly Specification<TEntity> _left;
    private readonly Specification<TEntity> _right;

    public OrSpecification(Specification<TEntity> left, Specification<TEntity> right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        _left = left;
        _right = right;
    }

    public override Expression<Func<TEntity, bool>> ToExpression()
    {
        var leftExpression = _left.ToExpression();
        var rightExpression = _right.ToExpression();

        var parameter = Expression.Parameter(typeof(TEntity));
        var combinedExpression = Expression.Lambda<Func<TEntity, bool>>(
            Expression.OrElse(
                Expression.Invoke(leftExpression, parameter),
                Expression.Invoke(rightExpression, parameter)
            ),
            parameter
        );

        return combinedExpression;
    }
}

/// <summary>
/// Negates a specification.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public class NotSpecification<TEntity> : Specification<TEntity> where TEntity : class, IEntity
{
    private readonly Specification<TEntity> _specification;

    public NotSpecification(Specification<TEntity> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);
        _specification = specification;
    }

    public override Expression<Func<TEntity, bool>> ToExpression()
    {
        var expression = _specification.ToExpression();
        var parameter = Expression.Parameter(typeof(TEntity));

        var notExpression = Expression.Lambda<Func<TEntity, bool>>(
            Expression.Not(Expression.Invoke(expression, parameter)),
            parameter
        );

        return notExpression;
    }
}
