using System.Linq.Expressions;
using AutoMapper;
using Bonyan.Layer.Domain.ValueObjects;

namespace Bonyan.AutoMapper;

public static class AutoMapperExtensions
{
    public static IMappingExpression<TSource, TDestination> MapBusinessId<TSource, TDestination, TBusinessId, TKey>(
        this IMappingExpression<TSource, TDestination> mappingExpression,
        Expression<Func<TSource, BonBusinessId<TBusinessId, TKey>>> sourceMember,
        Expression<Func<TDestination, TKey>> destinationMember)
        where TBusinessId : BonBusinessId<TBusinessId, TKey>, new()
    {
        // ایجاد عبارت برای دسترسی به ویژگی Value
        var valueProperty = Expression.Property(sourceMember.Body, nameof(BonBusinessId<TBusinessId, TKey>.Value));

        // ساختن Lambda Expression جدید
        var lambda = Expression.Lambda<Func<TSource, TKey>>(valueProperty, sourceMember.Parameters);

        // استفاده از عبارت جدید در نگاشت
        mappingExpression.ForMember(destinationMember, opt => opt.MapFrom(lambda));

        return mappingExpression;
    }

    public static IMappingExpression<TDestination, TSource> MapBusinessIdReverse<TDestination, TSource, TBusinessId, TKey>(
        this IMappingExpression<TDestination, TSource> mappingExpression,
        Expression<Func<TDestination, TKey>> sourceMember,
        Expression<Func<TSource, BonBusinessId<TBusinessId, TKey>>> destinationMember)
        where TBusinessId : BonBusinessId<TBusinessId, TKey>, new()
    {
        // دسترسی به متد FromValue
        var fromValueMethod = typeof(BonBusinessId<TBusinessId, TKey>).GetMethod(nameof(BonBusinessId<TBusinessId, TKey>.FromValue), new[] { typeof(TKey) });

        // ایجاد عبارت برای ساختن نمونه جدید از BonBusinessId
        var newBusinessIdExpr = Expression.Call(fromValueMethod, sourceMember.Body);

        // ساختن Lambda Expression جدید
        var lambda = Expression.Lambda<Func<TDestination, BonBusinessId<TBusinessId, TKey>>>(newBusinessIdExpr, sourceMember.Parameters);

        // استفاده از عبارت جدید در نگاشت
        mappingExpression.ForMember(destinationMember, opt => opt.MapFrom(lambda));

        return mappingExpression;
    }
}
