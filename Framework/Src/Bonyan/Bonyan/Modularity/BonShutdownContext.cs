namespace Bonyan.Modularity;

/// <summary>
/// Context for module shutdown phase, providing access to services during cleanup.
/// </summary>
public class BonShutdownContext : BonContextBase
{
    /// <summary>
    /// Initializes a new instance of <see cref="BonShutdownContext"/>.
    /// </summary>
    /// <param name="services">The service provider for dependency resolution.</param>
    public BonShutdownContext(IServiceProvider services)
        : base(services)
    {
    }
}

