using System.Linq.Expressions;
using Bonyan.Layer.Domain.Entity;
using Bonyan.Layer.Domain.Specification.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Bonyan.Layer.Domain.Specifications;

public static class IncludeSpecificationContextExtensions
{
    /// <summary>
    /// Applies ThenInclude for a reference type navigation property.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity being queried.</typeparam>
    /// <typeparam name="TPreviousProperty">The type of the previously included property.</typeparam>
    /// <typeparam name="TNextProperty">The type of the next property to include.</typeparam>
    /// <param name="context">The context containing the query.</param>
    /// <param name="navigationPropertyPath">The navigation property to include.</param>
    /// <returns>An updated BonIncludeBonSpecificationContext with the ThenInclude applied.</returns>
    public static BonIncludeBonSpecificationContext<TEntity, TNextProperty> ThenInclude<TEntity, TPreviousProperty, TNextProperty>(
        this IBonIncludeBonSpecificationContext<TEntity, TPreviousProperty> context,
        Expression<Func<TPreviousProperty, TNextProperty>> navigationPropertyPath)
        where TEntity : class, IBonEntity
    {
        // Apply ThenInclude to the current query
        var query = ((IIncludableQueryable<TEntity, TPreviousProperty>)context.Query).ThenInclude(navigationPropertyPath);

        // Return a new BonIncludeBonSpecificationContext with the updated query
        return new BonIncludeBonSpecificationContext<TEntity, TNextProperty>(query);
    }

    /// <summary>
    /// Applies ThenInclude for a collection type navigation property.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity being queried.</typeparam>
    /// <typeparam name="TPreviousProperty">The type of the previously included property.</typeparam>
    /// <typeparam name="TNextProperty">The type of the next property to include.</typeparam>
    /// <param name="context">The context containing the query.</param>
    /// <param name="navigationPropertyPath">The navigation property to include.</param>
    /// <returns>An updated BonIncludeBonSpecificationContext with the ThenInclude applied.</returns>
    public static BonIncludeBonSpecificationContext<TEntity, TNextProperty> ThenInclude<TEntity, TPreviousProperty, TNextProperty>(
        this IBonIncludeBonSpecificationContext<TEntity, IEnumerable<TPreviousProperty>> context,
        Expression<Func<TPreviousProperty, TNextProperty>> navigationPropertyPath)
        where TEntity : class, IBonEntity
    {
        // Apply ThenInclude for collections to the current query
        var query = ((IIncludableQueryable<TEntity, IEnumerable<TPreviousProperty>>)context.Query).ThenInclude(navigationPropertyPath);

        // Return a new BonIncludeBonSpecificationContext with the updated query
        return new BonIncludeBonSpecificationContext<TEntity, TNextProperty>(query);
    }

    public static BonIncludeBonSpecificationContext<TEntity, TNextProperty> ThenInclude<TEntity, TPreviousProperty, TNextProperty>(
        this IBonIncludeBonSpecificationContext<TEntity, ICollection<TPreviousProperty>> context,
        Expression<Func<TPreviousProperty, TNextProperty>> navigationPropertyPath)
        where TEntity : class, IBonEntity
    {
        // Apply ThenInclude for collections to the current query
        var query = ((IIncludableQueryable<TEntity, ICollection<TPreviousProperty>>)context.Query).ThenInclude(navigationPropertyPath);

        // Return a new BonIncludeBonSpecificationContext with the updated query
        return new BonIncludeBonSpecificationContext<TEntity, TNextProperty>(query);
    }
}
