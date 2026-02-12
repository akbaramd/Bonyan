namespace Bonyan.DependencyInjection;

/// <summary>
/// List of actions invoked when a service is registered; can disable class interceptors globally or per type.
/// </summary>

public class BonServiceRegistrationActionList : List<Action<IOnServiceRegisteredContext>>
{
    public bool IsClassInterceptorsDisabled { get; set; }

    public IClassInterceptorsSelectorList DisabledClassInterceptorsSelectors { get; }

    public BonServiceRegistrationActionList()
    {
        DisabledClassInterceptorsSelectors = new ClassInterceptorsSelectorList();
    }
}
public class ClassInterceptorsSelectorList : List<NamedTypeSelector>, IClassInterceptorsSelectorList
{

}
public interface IClassInterceptorsSelectorList : IList<NamedTypeSelector>
{

}
public class ServiceRegistrationActionList : List<Action<IOnServiceRegisteredContext>>
{
    public bool IsClassInterceptorsDisabled { get; set; }

    public IClassInterceptorsSelectorList DisabledClassInterceptorsSelectors { get; }

    public ServiceRegistrationActionList()
    {
        DisabledClassInterceptorsSelectors = new ClassInterceptorsSelectorList();
    }
}

public class NamedTypeSelector
{
    /// <summary>
    /// Name of the selector.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Predicate.
    /// </summary>
    public Func<Type, bool> Predicate { get; set; }

    /// <summary>
    /// Creates new <see cref="NamedTypeSelector"/> object.
    /// </summary>
    /// <param name="name">Name</param>
    /// <param name="predicate">Predicate</param>
    public NamedTypeSelector(string name, Func<Type, bool> predicate)
    {
        Name = name;
        Predicate = predicate;
    }
}