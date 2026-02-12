using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.DependencyInjection;

/// <summary>
/// Holds actions that run when a service is activated; keyed by service descriptor.
/// </summary>
public class BonServiceActivatedActionList
{
    private readonly List<(Type? serviceType, Type? implementationType, object? key, Action<IOnServiceActivatedContext> action)> _actions = new();

    /// <summary>
    /// Gets actions for the given descriptor (matching service type, implementation type, and key).
    /// </summary>
    public IEnumerable<Action<IOnServiceActivatedContext>> GetActions(ServiceDescriptor serviceDescriptor)
    {
        foreach (var (serviceType, implementationType, key, action) in _actions)
        {
            if ((serviceType == null || serviceType == serviceDescriptor.ServiceType) &&
                (implementationType == null || implementationType == serviceDescriptor.ImplementationType) &&
                (key == null || key.Equals(serviceDescriptor.ServiceKey)))
            {
                yield return action;
            }
        }
    }

    /// <summary>
    /// Adds an action that runs when a service of type <typeparamref name="T"/> is activated.
    /// </summary>
    public void Add<T>(Action<IOnServiceActivatedContext> action)
    {
        _actions.Add((typeof(T), null, null, action));
    }

    /// <summary>
    /// Adds an action that runs when any service matching the descriptor is activated.
    /// </summary>
    public void Add(ServiceDescriptor serviceDescriptor, Action<IOnServiceActivatedContext> action)
    {
        _actions.Add((serviceDescriptor.ServiceType, serviceDescriptor.ImplementationType, serviceDescriptor.ServiceKey, action));
    }
}
