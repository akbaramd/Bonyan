namespace Bonyan.Modularity;

/// <summary>
/// Used to define dependencies of a type.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependOnAttribute : Attribute, IDependedTypesProvider
{
  public Type[] DependedTypes { get; }

  public DependOnAttribute(params Type[]? dependedTypes)
  {
    DependedTypes = dependedTypes ?? Type.EmptyTypes;
  }

  public virtual Type[] GetDependedTypes()
  {
    return DependedTypes;
  }
}
