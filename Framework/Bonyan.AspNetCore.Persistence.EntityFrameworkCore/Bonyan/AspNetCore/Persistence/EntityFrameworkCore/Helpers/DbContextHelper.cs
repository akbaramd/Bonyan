using System.Reflection;
using Bonyan.DomainDrivenDesign.Domain.Entities;
using Bonyan.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Bonyan.AspNetCore.Persistence.EntityFrameworkCore.Helpers;
internal static class DbContextHelper
{
  public static IEnumerable<Type> GetEntityTypes(Type dbContextType)
  {
    return
      from property in dbContextType.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance)
      where
        ReflectionHelper.IsAssignableToGenericType(property.PropertyType, typeof(DbSet<>)) &&
        typeof(IEntity).IsAssignableFrom(property.PropertyType.GenericTypeArguments[0])
      select property.PropertyType.GenericTypeArguments[0];
  }
}
