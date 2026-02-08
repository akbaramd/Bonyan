namespace Bonyan.DependencyInjection;

/// <summary>
/// List of actions invoked when a service is registered; can disable class interceptors globally.
/// </summary>
public class BonServiceRegistrationActionList : List<Action<IOnServiceRegisteredContext>>
{
    /// <summary>When true, class interceptors are not applied for conventional registration.</summary>
    public bool IsClassInterceptorsDisabled { get; set; }
}
