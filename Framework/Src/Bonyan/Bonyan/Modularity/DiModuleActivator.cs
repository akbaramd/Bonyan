using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Bonyan.Modularity;

/// <summary>
/// Creates module instances using dependency injection (ActivatorUtilities).
/// Part of the microkernel architecture - modules are created via DI, not raw Activator.
/// </summary>
public sealed class DiModuleActivator : IModuleActivator
{
    private readonly ILogger<DiModuleActivator>? _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="DiModuleActivator"/>.
    /// </summary>
    /// <param name="logger">Optional logger for diagnostic information.</param>
    public DiModuleActivator(ILogger<DiModuleActivator>? logger = null)
    {
        _logger = logger;
    }

    /// <summary>
    /// Creates and registers a module instance using dependency injection.
    /// </summary>
    /// <param name="services">Service collection to register the module in.</param>
    /// <param name="moduleType">Type of module to create.</param>
    /// <returns>The created module instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown if module creation fails.</exception>
    public IBonModule CreateAndRegister(IServiceCollection services, Type moduleType)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));
        if (moduleType == null)
            throw new ArgumentNullException(nameof(moduleType));

        // Validate type
        if (!typeof(IBonModule).IsAssignableFrom(moduleType))
        {
            throw new InvalidOperationException(
                $"{moduleType.FullName} must implement {nameof(IBonModule)}.");
        }

        if (moduleType.IsAbstract)
        {
            throw new InvalidOperationException(
                $"{moduleType.FullName} cannot be abstract.");
        }

        try
        {
            // Register module for DI creation (supports constructor injection)
            // The module will be created from the final service provider during lifecycle
            services.TryAddSingleton(moduleType, sp =>
            {
                try
                {
                    var instance = (IBonModule)ActivatorUtilities.CreateInstance(sp, moduleType);
                    return instance;
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Failed to create module instance: {ModuleType}", moduleType.FullName);
                    throw new InvalidOperationException(
                        $"Failed to create instance of module type {moduleType.FullName}. " +
                        "Ensure the type has a valid constructor and all dependencies are registered.", ex);
                }
            });

            // For immediate use during module loading, create instance with minimal dependencies
            // This maintains backward compatibility but modules should ideally be created from final provider
            // TODO: Refactor to lazy-load modules from final provider
            IBonModule instance;
            try
            {
                // Try to create with ActivatorUtilities if we have a provider, otherwise fallback to Activator
                var tempProvider = services.BuildServiceProvider();
                try
                {
                    instance = (IBonModule)ActivatorUtilities.CreateInstance(tempProvider, moduleType);
                }
                catch
                {
                    // Fallback for modules without DI dependencies
                    instance = (IBonModule)Activator.CreateInstance(moduleType)!;
                }
                tempProvider.Dispose();
            }
            catch
            {
                // Final fallback
                instance = (IBonModule)Activator.CreateInstance(moduleType)!;
            }
            
            if (instance == null)
            {
                throw new InvalidOperationException(
                    $"Failed to create instance of module type {moduleType.FullName}.");
            }
            
            return instance;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to create and register module: {ModuleType}", moduleType.FullName);
            throw;
        }
    }
}

