namespace System.Linq.Expressions;

/// <summary>
/// Provides extension methods for IQueryable to support conditional queries, dynamic ordering, and pagination.
/// </summary>
public static class QueryExtensions
{
    /// <summary>
    /// Applies a conditional filter to the query if the specified condition is true.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the query.</typeparam>
    /// <param name="source">The source queryable collection.</param>
    /// <param name="condition">The condition to determine if the filter should be applied.</param>
    /// <param name="predicate">The filter expression to apply if the condition is true.</param>
    /// <returns>The filtered query if the condition is true; otherwise, the original query.</returns>
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool condition, Expression<Func<T, bool>> predicate)
    {
        return condition ? source.Where(predicate) : source;
    }

    /// <summary>
    /// Dynamically orders the query by a specified property name in ascending or descending order.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the query.</typeparam>
    /// <param name="source">The source queryable collection.</param>
    /// <param name="propertyName">The name of the property to order by.</param>
    /// <param name="ascending">Indicates whether to order ascending (true) or descending (false).</param>
    /// <returns>The ordered query.</returns>
    /// <exception cref="ArgumentException">Thrown if the property name does not exist.</exception>
    public static IQueryable<T> OrderByProperty<T>(this IQueryable<T> source, string propertyName, bool ascending = true)
    {
        var param = Expression.Parameter(typeof(T), "p");
        var prop = Expression.Property(param, propertyName);
        var exp = Expression.Lambda(prop, param);
        var method = ascending ? "OrderBy" : "OrderByDescending";
        var types = new[] { source.ElementType, prop.Type };
        var mce = Expression.Call(typeof(Queryable), method, types, source.Expression, exp);
        return source.Provider.CreateQuery<T>(mce);
    }

    /// <summary>
    /// Applies secondary ordering to the query by a specified property name in ascending or descending order.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the query.</typeparam>
    /// <param name="source">The source queryable collection.</param>
    /// <param name="propertyName">The name of the property to apply secondary ordering by.</param>
    /// <param name="ascending">Indicates whether to order ascending (true) or descending (false).</param>
    /// <returns>The query with the secondary ordering applied.</returns>
    /// <exception cref="ArgumentException">Thrown if the property name does not exist.</exception>
    public static IQueryable<T> ThenOrderByProperty<T>(this IQueryable<T> source, string propertyName, bool ascending = true)
    {
        var param = Expression.Parameter(typeof(T), "p");
        var prop = Expression.Property(param, propertyName);
        var exp = Expression.Lambda(prop, param);
        var method = ascending ? "ThenBy" : "ThenByDescending";
        var types = new[] { source.ElementType, prop.Type };
        var mce = Expression.Call(typeof(Queryable), method, types, source.Expression, exp);
        return source.Provider.CreateQuery<T>(mce);
    }

    /// <summary>
    /// Applies pagination to the query by skipping and taking a specified number of elements.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the query.</typeparam>
    /// <param name="query">The source queryable collection.</param>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The number of elements per page.</param>
    /// <returns>The paginated query.</returns>
    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int page, int pageSize)
    {
        return query.Skip((page - 1) * pageSize).Take(pageSize);
    }
    
    
    /// <summary>
    /// Orders the query based on a string input such as "Id desc" or "Name asc".
    /// </summary>
    /// <typeparam name="T">The type of the elements in the query.</typeparam>
    /// <param name="source">The source queryable collection.</param>
    /// <param name="orderBy">The ordering string, e.g., "Id desc" or "Name asc".</param>
    /// <returns>The ordered query.</returns>
    /// <exception cref="ArgumentException">Thrown if the ordering string is not valid.</exception>
    public static IQueryable<T> OrderByString<T>(this IQueryable<T> source, string orderBy)
    {
      if (string.IsNullOrWhiteSpace(orderBy))
      {
        throw new ArgumentException("Order by string cannot be null or empty.", nameof(orderBy));
      }

      var orderParams = orderBy.Trim().Split(' ');
      var propertyName = orderParams[0];
      var isDescending = orderParams.Length > 1 && orderParams[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

      var param = Expression.Parameter(typeof(T), "p");
      var prop = Expression.Property(param, propertyName);
      var exp = Expression.Lambda(prop, param);

      var method = isDescending ? "OrderByDescending" : "OrderBy";
      var types = new[] { source.ElementType, prop.Type };
      var mce = Expression.Call(typeof(Queryable), method, types, source.Expression, exp);

      return source.Provider.CreateQuery<T>(mce);
    } 
}
