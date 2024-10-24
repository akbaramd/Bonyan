namespace Bonyan.Modularity.Attributes;

public class DependOnAttribute(params Type[] types) : Attribute
{
  public Type[] DependedTypes { get; set; } = types;
}
