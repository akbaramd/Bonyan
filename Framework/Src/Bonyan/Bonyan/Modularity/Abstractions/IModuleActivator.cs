using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Modularity.Abstractions;

/// <summary>
/// Creates module instances from the service provider using dependency injection.
/// Part of the microkernel core architecture.
/// </summary>
public interface IModuleActivator
{
    /// <summary>
    /// Creates and registers a module instance using dependency injection.
    /// </summary>
    /// <param name="services">Service collection to register the module in.</param>
    /// <param name="moduleType">Type of module to create.</param>
    /// <returns>The created module instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown if module creation fails.</exception>
    IBonModule CreateAndRegister(IServiceCollection services, Type moduleType);
}

