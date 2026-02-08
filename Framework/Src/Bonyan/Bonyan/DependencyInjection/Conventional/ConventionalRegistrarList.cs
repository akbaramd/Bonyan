namespace Bonyan.DependencyInjection;

/// <summary>
/// List of conventional registrars collected during module configuration.
/// Stored on <see cref="IServiceCollection"/> via object accessor; modules add registrars in OnConfigureAsync.
/// </summary>
public class ConventionalRegistrarList : List<IConventionalRegistrar>
{
}
