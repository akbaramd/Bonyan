using System.Reflection;
using Bonyan.Helpers;
using Bonyan.Layer.Domain.Entities;
using Bonyan.Layer.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.EntityFrameworkCore.Helpers;
internal static class DbContextHelper
{
  public static IEnumerable<Type> GetEntityTypes(Type dbContextType)
  {
    return
      from property in dbContextType.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance)
      where
        ReflectionHelper.IsAssignableToGenericType(property.PropertyType, typeof(DbSet<>)) &&
        typeof(IBonEntity).IsAssignableFrom(property.PropertyType.GenericTypeArguments[0])
      select property.PropertyType.GenericTypeArguments[0];
  }
}
