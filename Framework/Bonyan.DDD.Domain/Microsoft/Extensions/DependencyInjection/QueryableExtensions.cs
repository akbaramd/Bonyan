using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for IQueryable to apply conditional operations.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Conditionally includes a related entity in the query if the provider is from Entity Framework Core.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="query">The query to apply the include to.</param>
    /// <param name="includeExpression">The expression representing the related entity to include.</param>
    /// <returns>The query with the include applied if applicable; otherwise, the original query.</returns>
    public static IQueryable<TEntity> IncludeIfAvailable<TEntity>(this IQueryable<TEntity> query,
        Expression<Func<TEntity, object>> includeExpression) where TEntity : class
    {
        if (query.Provider is EntityQueryProvider)
        {
            return query.Include(includeExpression);
        }

        return query;
    }

    /// <summary>
    /// Conditionally filters the query based on a predicate if the condition is true.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="query">The query to apply the filter to.</param>
    /// <param name="condition">The condition that determines if the filter should be applied.</param>
    /// <param name="predicate">The predicate to filter the query.</param>
    /// <returns>The filtered query if the condition is true; otherwise, the original query.</returns>
    public static IQueryable<TEntity> WhereIf<TEntity>(this IQueryable<TEntity> query,
        bool condition, Expression<Func<TEntity, bool>> predicate) where TEntity : class
    {
        return condition ? query.Where(predicate) : query;
    }

    /// <summary>
    /// Conditionally orders the query by a key in ascending order if the condition is true.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TKey">The type of the key to order by.</typeparam>
    /// <param name="query">The query to apply the order to.</param>
    /// <param name="condition">The condition that determines if the order should be applied.</param>
    /// <param name="keySelector">The key selector for ordering.</param>
    /// <returns>The ordered query if the condition is true; otherwise, the original query.</returns>
    public static IQueryable<TEntity> OrderByIf<TEntity, TKey>(this IQueryable<TEntity> query,
        bool condition, Expression<Func<TEntity, TKey>> keySelector) where TEntity : class
    {
        return condition ? query.OrderBy(keySelector) : query;
    }

    /// <summary>
    /// Conditionally orders the query by a key in descending order if the condition is true.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TKey">The type of the key to order by.</typeparam>
    /// <param name="query">The query to apply the order to.</param>
    /// <param name="condition">The condition that determines if the order should be applied.</param>
    /// <param name="keySelector">The key selector for ordering.</param>
    /// <returns>The ordered query if the condition is true; otherwise, the original query.</returns>
    public static IQueryable<TEntity> OrderByDescendingIf<TEntity, TKey>(this IQueryable<TEntity> query,
        bool condition, Expression<Func<TEntity, TKey>> keySelector) where TEntity : class
    {
        return condition ? query.OrderByDescending(keySelector) : query;
    }

    /// <summary>
    /// Conditionally skips a specified number of elements in the query if the condition is true.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="query">The query to apply the skip to.</param>
    /// <param name="condition">The condition that determines if the skip should be applied.</param>
    /// <param name="count">The number of elements to skip.</param>
    /// <returns>The query with the skip applied if the condition is true; otherwise, the original query.</returns>
    public static IQueryable<TEntity> SkipIf<TEntity>(this IQueryable<TEntity> query,
        bool condition, int count) where TEntity : class
    {
        return condition ? query.Skip(count) : query;
    }

    /// <summary>
    /// Conditionally takes a specified number of elements from the query if the condition is true.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="query">The query to apply the take to.</param>
    /// <param name="condition">The condition that determines if the take should be applied.</param>
    /// <param name="count">The number of elements to take.</param>
    /// <returns>The query with the take applied if the condition is true; otherwise, the original query.</returns>
    public static IQueryable<TEntity> TakeIf<TEntity>(this IQueryable<TEntity> query,
        bool condition, int count) where TEntity : class
    {
        return condition ? query.Take(count) : query;
    }

    /// <summary>
    /// Conditionally applies pagination to the query.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="query">The query to apply pagination to.</param>
    /// <param name="condition">The condition that determines if pagination should be applied.</param>
    /// <param name="skipCount">The number of elements to skip.</param>
    /// <param name="takeCount">The number of elements to take.</param>
    /// <returns>The paginated query if the condition is true; otherwise, the original query.</returns>
    public static IQueryable<TEntity> PaginateIf<TEntity>(this IQueryable<TEntity> query,
        bool condition, int skipCount, int takeCount) where TEntity : class
    {
        return condition ? query.Skip(skipCount).Take(takeCount) : query;
    }

    /// <summary>
    /// Conditionally selects specific fields from the entity based on the provided selector expression.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="query">The query to apply the select to.</param>
    /// <param name="condition">The condition that determines if the select should be applied.</param>
    /// <param name="selector">The expression that selects specific fields.</param>
    /// <returns>The query with the select applied if the condition is true; otherwise, the original query.</returns>
    public static IQueryable<TResult> SelectIf<TEntity, TResult>(this IQueryable<TEntity> query,
        bool condition, Expression<Func<TEntity, TResult>> selector) where TEntity : class
    {
        return condition ? query.Select(selector) : query.Cast<TResult>();
    }
    
    /// <summary>
    /// Conditionally applies a GroupBy operation to the query.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TKey">The type of the key to group by.</typeparam>
    /// <param name="query">The query to apply the GroupBy to.</param>
    /// <param name="condition">The condition that determines if the GroupBy should be applied.</param>
    /// <param name="keySelector">The key selector for grouping.</param>
    /// <returns>The grouped query if the condition is true; otherwise, the original query.</returns>
    public static IQueryable<IGrouping<TKey, TEntity>> GroupByIf<TEntity, TKey>(this IQueryable<TEntity> query,
      bool condition, Expression<Func<TEntity, TKey>> keySelector) where TEntity : class
    {
      if (condition)
      {
        return query.GroupBy(keySelector);
      }

      // To satisfy the return type, the default value must match the TKey type's nullability
      TKey defaultKey = default!;

      return query
        .GroupBy(e => defaultKey);  // Use the non-nullable defaultKey
    }

    /// <summary>
    /// Conditionally applies a Distinct operation to the query.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="query">The query to apply the Distinct to.</param>
    /// <param name="condition">The condition that determines if the Distinct should be applied.</param>
    /// <returns>The distinct query if the condition is true; otherwise, the original query.</returns>
    public static IQueryable<TEntity> DistinctIf<TEntity>(this IQueryable<TEntity> query,
        bool condition) where TEntity : class
    {
        return condition ? query.Distinct() : query;
    }

    /// <summary>
    /// Conditionally applies a FirstOrDefault operation to the query.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="query">The query to apply the FirstOrDefault to.</param>
    /// <param name="condition">The condition that determines if the FirstOrDefault should be applied.</param>
    /// <returns>The first element of the query if the condition is true; otherwise, the default value of the entity type.</returns>
    public static TEntity? FirstOrDefaultIf<TEntity>(this IQueryable<TEntity> query,
        bool condition) where TEntity : class
    {
        return condition ? query.FirstOrDefault() : default;
    }
}
