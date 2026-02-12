namespace Bonyan.DependencyInjection;

/// <summary>
/// Context passed when a service instance has been activated (created by the container).
/// </summary>
public interface IOnServiceActivatedContext
{
    /// <summary>The activated service instance.</summary>
    object Instance { get; }
}
