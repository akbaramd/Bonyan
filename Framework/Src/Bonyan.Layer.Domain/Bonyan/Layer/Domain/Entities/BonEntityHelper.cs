using System.Linq.Expressions;
using System.Reflection;
using Bonyan.Core;
using Bonyan.Exceptions;
using Bonyan.Helpers;
using Bonyan.Layer.Domain.Entity;
using Bonyan.Layer.Domain.ValueObjects;
using Bonyan.MultiTenant;
using JetBrains.Annotations;

namespace Bonyan.Layer.Domain.Entities;

/// <summary>
///     Helper methods for entities.
/// </summary>
public static class BonEntityHelper
{
    public static Func<Type, bool> IsValueObjectPredicate = type => typeof(BonValueObject).IsAssignableFrom(type);

    /// <summary>
    /// Checks if the entity implements multi-tenancy.
    /// </summary>
    public static bool IsMultiTenant<TEntity>()
        where TEntity : IBonEntity
    {
        return IsMultiTenant(typeof(TEntity));
    }

    public static bool IsMultiTenant(Type type)
    {
        return typeof(IBonMultiTenant).IsAssignableFrom(type);
    }

    /// <summary>
    /// Checks if two entities are equal.
    /// </summary>
    public static bool EntityEquals(IBonEntity? entity1, IBonEntity? entity2)
    {
        if (entity1 == null || entity2 == null) return false;

        // Same instances must be considered as equal
        if (ReferenceEquals(entity1, entity2)) return true;

        // Must have a IS-A relation of types or must be the same type
        var typeOfEntity1 = entity1.GetType();
        var typeOfEntity2 = entity2.GetType();
        if (!typeOfEntity1.IsAssignableFrom(typeOfEntity2) && !typeOfEntity2.IsAssignableFrom(typeOfEntity1))
            return false;

        // Different tenants may have an entity with the same Id.
        if (entity1 is IBonMultiTenant tenantEntity1 && entity2 is IBonMultiTenant tenantEntity2)
        {
            var tenant1Id = tenantEntity1.TenantId;
            var tenant2Id = tenantEntity2.TenantId;

            if (tenant1Id != tenant2Id)
            {
                if (tenant1Id == null || tenant2Id == null) return false;

                if (!tenant1Id.Equals(tenant2Id)) return false;
            }
        }

        // Transient objects are not considered equal
        if (HasDefaultKey(entity1) && HasDefaultKey(entity2)) return false;

        // Compare single keys
        var entity1Key = entity1.GetKey();
        var entity2Key = entity2.GetKey();

        if (entity1Key == null || entity2Key == null) return false;
        if (TypeHelper.IsDefaultValue(entity1Key) && TypeHelper.IsDefaultValue(entity2Key)) return false;

        return entity1Key.Equals(entity2Key);
    }

    public static bool IsEntity([NotNull] Type type)
    {
        Check.NotNull(type, nameof(type));
        return typeof(IBonEntity).IsAssignableFrom(type);
    }

    public static bool IsValueObject([NotNull] Type type)
    {
        Check.NotNull(type, nameof(type));
        return IsValueObjectPredicate(type);
    }

    public static bool IsValueObject(object? obj)
    {
        return obj != null && IsValueObject(obj.GetType());
    }

    public static void CheckEntity([NotNull] Type type)
    {
        Check.NotNull(type, nameof(type));
        if (!IsEntity(type))
            throw new BonException(
                $"Given {nameof(type)} is not an entity: {type.AssemblyQualifiedName}. It must implement {typeof(IBonEntity).AssemblyQualifiedName}.");
    }

    public static bool IsEntityWithId([NotNull] Type type)
    {
        foreach (var interfaceType in type.GetInterfaces())
            if (interfaceType.GetTypeInfo().IsGenericType &&
                interfaceType.GetGenericTypeDefinition() == typeof(IBonEntity<>))
                return true;

        return false;
    }

    public static bool HasDefaultId<TKey>(IBonEntity<TKey> bonEntity)
    {
        if (EqualityComparer<TKey>.Default.Equals(bonEntity.Id, default!)) return true;

        // Workaround for EF Core since it sets int/long to min value when attaching to DbContext
        if (typeof(TKey) == typeof(int)) return Convert.ToInt32(bonEntity.Id) <= 0;

        if (typeof(TKey) == typeof(long)) return Convert.ToInt64(bonEntity.Id) <= 0;

        return false;
    }

    private static bool IsDefaultKeyValue(object? value)
    {
        if (value == null) return true;

        var type = value.GetType();

        // Workaround for EF Core since it sets int/long to min value when attaching to DbContext
        if (type == typeof(int)) return Convert.ToInt32(value) <= 0;

        if (type == typeof(long)) return Convert.ToInt64(value) <= 0;

        return TypeHelper.IsDefaultValue(value);
    }

    public static bool HasDefaultKey([NotNull] IBonEntity bonEntity)
    {
        Check.NotNull(bonEntity, nameof(bonEntity));
        var key = bonEntity.GetKey();
        return IsDefaultKeyValue(key);
    }

    /// <summary>
    /// Tries to find the primary key type of the given entity type.
    /// </summary>
    public static Type? FindPrimaryKeyType<TEntity>()
        where TEntity : IBonEntity
    {
        return FindPrimaryKeyType(typeof(TEntity));
    }

    /// <summary>
    /// Tries to find the primary key type of the given entity type.
    /// </summary>
    public static Type? FindPrimaryKeyType([NotNull] Type entityType)
    {
        Check.NotNull(entityType, nameof(entityType));
        if (!typeof(IBonEntity).IsAssignableFrom(entityType))
            throw new BonException(
                $"Given {nameof(entityType)} is not an entity. It should implement {typeof(IBonEntity).AssemblyQualifiedName}!");

        foreach (var interfaceType in entityType.GetTypeInfo().GetInterfaces())
            if (interfaceType.GetTypeInfo().IsGenericType &&
                interfaceType.GetGenericTypeDefinition() == typeof(IBonEntity<>))
                return interfaceType.GenericTypeArguments[0];

        return null;
    }

    public static Expression<Func<TEntity, bool>> CreateEqualityExpressionForId<TEntity, TKey>(TKey id)
        where TEntity : IBonEntity<TKey>
    {
        var lambdaParam = Expression.Parameter(typeof(TEntity));
        var leftExpression = Expression.PropertyOrField(lambdaParam, "Id");
        var idValue = Convert.ChangeType(id, typeof(TKey));
        Expression<Func<object?>> closure = () => idValue;
        var rightExpression = Expression.Convert(closure.Body, leftExpression.Type);
        var lambdaBody = Expression.Equal(leftExpression, rightExpression);
        return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
    }

    public static void TrySetId<TKey>(
        IBonEntity<TKey> bonEntity,
        Func<TKey> idFactory,
        bool checkForDisableIdGenerationAttribute = false)
    {
        ObjectHelper.TrySetProperty(
            bonEntity,
            x => x.Id,
            idFactory,
            checkForDisableIdGenerationAttribute
                ? new[] { typeof(DisableIdGenerationAttribute) }
                : Array.Empty<Type>());
    }

    public static void TrySetTenantId(IBonEntity bonEntity)
    {
        if (bonEntity is not IBonMultiTenant multiTenantEntity) return;

        var tenantId = AsyncLocalCurrentTenantAccessor.Instance.Current?.TenantId;
        if (tenantId == multiTenantEntity.TenantId) return;

        ObjectHelper.TrySetProperty(
            multiTenantEntity,
            x => x.TenantId,
            () => tenantId
        );
    }

    public static void TrySetTenantId(IBonEntity bonEntity, Guid? tenantId)
    {
        if (bonEntity is not IBonMultiTenant multiTenantEntity) return;

        if (tenantId == multiTenantEntity.TenantId) return;

        ObjectHelper.TrySetProperty(
            multiTenantEntity,
            x => x.TenantId,
            () => tenantId
        );
    }
}
