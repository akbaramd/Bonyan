using Bonyan.Core;

namespace Bonyan.DependencyInjection;

/// <summary>
/// Default implementation of <see cref="IOnServiceActivatedContext"/>.
/// </summary>
public class OnServiceActivatedContext : IOnServiceActivatedContext
{
    public object Instance { get; }

    public OnServiceActivatedContext(object instance)
    {
        Instance = Check.NotNull(instance, nameof(instance));
    }
}
