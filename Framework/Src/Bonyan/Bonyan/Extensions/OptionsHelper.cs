using System.Reflection;

namespace Bonyan.Extensions
{
  public static class OptionsHelper
  {
    /// <summary>
    /// Copies all writable properties from the source object to the target object.
    /// Only properties with matching names and compatible types are copied.
    /// </summary>
    /// <typeparam name="T">Type of the target object.</typeparam>
    /// <typeparam name="TU">Type of the source object.</typeparam>
    /// <param name="target">The target object to which properties will be copied.</param>
    /// <param name="source">The source object from which properties will be copied.</param>
    public static void CopyProperties<T, TU>(T target, TU source)
    {
      if (target == null)
        throw new ArgumentNullException(nameof(target));
      if (source == null)
        throw new ArgumentNullException(nameof(source));

      PropertyInfo[] targetProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
      PropertyInfo[] sourceProperties = typeof(TU).GetProperties(BindingFlags.Public | BindingFlags.Instance);

      foreach (var targetProp in targetProperties)
      {
        if (!targetProp.CanWrite)
          continue;

        var sourceProp = Array.Find(sourceProperties, p => p.Name == targetProp.Name && p.PropertyType == targetProp.PropertyType);
        if (sourceProp != null && sourceProp.CanRead)
        {
          var value = sourceProp.GetValue(source);
          targetProp.SetValue(target, value);
        }
      }
    }
  }
}
