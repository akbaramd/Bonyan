using System.Linq.Expressions;

namespace Bonyan.Layer.Domain.Abstractions;

public interface IBonSpecificationContext<T> where T : class
{
  IQueryable<T> Query { get; }

  // Method to add criteria (filtering)
  void AddCriteria(Expression<Func<T, bool>> criteria);

  // Method to add ordering (ascending)
  void ApplyOrderBy<TKey>(Expression<Func<T, TKey>> keySelector);

  // Method to add ordering (descending)
  void ApplyOrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector);

  
  // Method to apply includes (eager loading)
  IBonIncludeBonSpecificationContext<T, TProperty> AddInclude<TProperty>(Expression<Func<T, TProperty>> include);

  // New methods
  // Apply where entity property matches any value from a list
  void ApplyWhereIn<TKey>(Expression<Func<T, TKey>> keySelector, IEnumerable<TKey> values);

  // Apply distinct
  void ApplyDistinct();

  // Apply group by
  void ApplyGroupBy<TKey>(Expression<Func<T, TKey>> keySelector);

  // Get max value
  TKey? ApplyMax<TKey>(Expression<Func<T, TKey>> keySelector);

  // Get min value
  TKey? ApplyMin<TKey>(Expression<Func<T, TKey>> keySelector);
}