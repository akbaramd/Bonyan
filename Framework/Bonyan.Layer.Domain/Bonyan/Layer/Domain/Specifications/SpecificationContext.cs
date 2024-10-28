using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Bonyan.Layer.Domain.Specifications;

public class SpecificationContext<T> : ISpecificationContext<T> where T : class
{
  public IQueryable<T> Query { get; set; }

  public SpecificationContext(IQueryable<T> query)
  {
    Query = query;
  }

  public void AddCriteria(Expression<Func<T, bool>> criteria)
  {
    Query = Query.Where(criteria);
  }

  public void ApplyOrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
  {
    Query = Query.OrderBy(keySelector);
  }

  public void ApplyOrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
  {
    Query = Query.OrderByDescending(keySelector);
  }



  public IIncludeSpecificationContext<T, TProperty> AddInclude<TProperty>(Expression<Func<T, TProperty>> include)
  {
    // Correctly update the Query by applying the Include
    Query = Query.Include(include);

    // Return a new IncludeSpecificationContext to allow ThenInclude calls if needed
    return new IncludeSpecificationContext<T, TProperty>((IIncludableQueryable<T, TProperty>)Query);
  }

  // Apply filtering based on a list of values (similar to SQL IN clause)
  public void ApplyWhereIn<TKey>(Expression<Func<T, TKey>> keySelector, IEnumerable<TKey> values)
  {
    Query = Query.Where(e => values.Contains(keySelector.Compile()(e)));
  }

  // Apply distinct to remove duplicates
  public void ApplyDistinct()
  {
    Query = Query.Distinct();
  }

  // Apply grouping based on a key selector
  public void ApplyGroupBy<TKey>(Expression<Func<T, TKey>> keySelector)
  {
    Query = Query.GroupBy(keySelector).SelectMany(g => g);
  }

  // Get the maximum value of a property
  public TKey? ApplyMax<TKey>(Expression<Func<T, TKey>> keySelector)
  {
    return Query.Max(keySelector);
  }

  // Get the minimum value of a property
  public TKey? ApplyMin<TKey>(Expression<Func<T, TKey>> keySelector)
  {
    return Query.Min(keySelector);
  }
}
