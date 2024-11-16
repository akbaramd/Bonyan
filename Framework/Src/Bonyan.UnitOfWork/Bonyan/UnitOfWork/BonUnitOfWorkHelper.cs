using System.Reflection;
using Bonyan.Core;
using JetBrains.Annotations;

namespace Bonyan.UnitOfWork;

public static class BonUnitOfWorkHelper
{
    public static bool IsUnitOfWorkType(TypeInfo implementationType)
    {
        //Explicitly defined UnitOfWorkAttribute
        if (HasUnitOfWorkAttribute(implementationType) || AnyMethodHasUnitOfWorkAttribute(implementationType))
        {
            return true;
        }

        //Conventional classes
        if (typeof(IBonUnitOfWorkEnabled).GetTypeInfo().IsAssignableFrom(implementationType))
        {
            return true;
        }

        return false;
    }

    public static bool IsUnitOfWorkMethod([NotNull] MethodInfo methodInfo, out BonUnitOfWorkAttribute? unitOfWorkAttribute)
    {
        Check.NotNull(methodInfo, nameof(methodInfo));

        //Method declaration
        var attrs = methodInfo.GetCustomAttributes(true).OfType<BonUnitOfWorkAttribute>().ToArray();
        if (attrs.Any())
        {
            unitOfWorkAttribute = attrs.First();
            return !unitOfWorkAttribute.IsDisabled;
        }

        if (methodInfo.DeclaringType != null)
        {
            //Class declaration
            attrs = methodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes(true).OfType<BonUnitOfWorkAttribute>().ToArray();
            if (attrs.Any())
            {
                unitOfWorkAttribute = attrs.First();
                return !unitOfWorkAttribute.IsDisabled;
            }

            //Conventional classes
            if (typeof(IBonUnitOfWorkEnabled).GetTypeInfo().IsAssignableFrom(methodInfo.DeclaringType))
            {
                unitOfWorkAttribute = null;
                return true;
            }
        }

        unitOfWorkAttribute = null;
        return false;
    }

    public static BonUnitOfWorkAttribute? GetUnitOfWorkAttributeOrNull(MethodInfo methodInfo)
    {
        var attrs = methodInfo.GetCustomAttributes(true).OfType<BonUnitOfWorkAttribute>().ToArray();
        if (attrs.Length > 0)
        {
            return attrs[0];
        }

        if (methodInfo.DeclaringType != null)
        {
            attrs = methodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes(true).OfType<BonUnitOfWorkAttribute>().ToArray();
            if (attrs.Length > 0)
            {
                return attrs[0];
            }
        }
        
        return null;
    }

    private static bool AnyMethodHasUnitOfWorkAttribute(TypeInfo implementationType)
    {
        return implementationType
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Any(HasUnitOfWorkAttribute);
    }

    private static bool HasUnitOfWorkAttribute(MemberInfo methodInfo)
    {
        return methodInfo.IsDefined(typeof(BonUnitOfWorkAttribute), true);
    }
}
