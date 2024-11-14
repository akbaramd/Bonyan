using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan;

/// <summary>
/// Manages the modular application lifecycle, including module loading, configuration, and initialization.
/// </summary>
/// <typeparam name="TModule">The type of the root module.</typeparam>
public class BonModularityApplication<TModule> : IBonModularityApplication where TModule : IBonModule
{
    private readonly IServiceCollection _serviceCollection;
    private readonly IBonModuleAccessor _bonModuleAccessor;

    /// <summary>
    /// Initializes a new instance of <see cref="BonModularityApplication{TModule}"/> with core services.
    /// </summary>
    /// <param name="serviceCollection">The service collection to which dependencies are added.</param>
    public BonModularityApplication(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
        
        _bonModuleAccessor = new BonModuleAccessor();
        var moduleManager = new ModuleManager(_bonModuleAccessor);

        // Register core services
        _serviceCollection.AddSingleton(_bonModuleAccessor);
        _serviceCollection.AddSingleton(moduleManager);
        _serviceCollection.AddSingleton<IModuleLoader>(moduleManager);
        _serviceCollection.AddSingleton<IBonModuleConfigurator>(this);
        _serviceCollection.AddSingleton<IBonModuleInitializer>(this);

        _bonModuleAccessor.AddModule(new ModuleInfo(typeof(BonMasterModule)));
        moduleManager.LoadModules(typeof(TModule));
        
        ServiceProvider = _serviceCollection.BuildServiceProvider();
    }

    /// <summary>
    /// Gets the service provider for dependency resolution.
    /// </summary>
    public IServiceProvider ServiceProvider { get; private set; }

    /// <summary>
    /// Gets all loaded modules.
    /// </summary>
    public IEnumerable<ModuleInfo> Modules => _bonModuleAccessor.GetAllModules();

    /// <summary>
    /// Configures all modules by invoking pre-configuration, configuration, and post-configuration phases.
    /// </summary>
    public async Task ConfigureModulesAsync()
    {
        using var serviceProvider = _serviceCollection.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var context = new BonConfigurationContext(_serviceCollection, configuration);

        foreach (var module in Modules)
        {
            if (module.Instance != null) module.Instance.Services = _serviceCollection;
        }
        
        // Execute configuration phases in order
        await ExecuteModulePhaseAsync(context, (module, ctx) => module.OnPreConfigureAsync(ctx));
        await ExecuteModulePhaseAsync(context, (module, ctx) => module.OnConfigureAsync(ctx));
        await ExecuteModulePhaseAsync(context, (module, ctx) => module.OnPostConfigureAsync(ctx));
    }

    /// <summary>
    /// Initializes all modules by invoking pre-initialization, initialization, and post-initialization phases.
    /// </summary>
    /// <param name="serviceProvider">The application service provider.</param>
    public async Task InitializeModulesAsync(IServiceProvider serviceProvider)
    {
        if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var context = new ServiceInitializationContext(serviceProvider, configuration);

        // Execute initialization phases in order
        await ExecuteModulePhaseAsync(context, (module, ctx) => module.OnPreInitializeAsync(ctx));
        await ExecuteModulePhaseAsync(context, (module, ctx) => module.OnInitializeAsync(ctx));
        await ExecuteModulePhaseAsync(context, (module, ctx) => module.OnPostInitializeAsync(ctx));
    }

    /// <summary>
    /// Executes a specific module phase for each module.
    /// </summary>
    /// <typeparam name="TContext">The context type for the module phase.</typeparam>
    /// <param name="context">The context to pass to each module phase.</param>
    /// <param name="phaseAction">The action to execute for each module phase.</param>
    private async Task ExecuteModulePhaseAsync<TContext>(
        TContext context, 
        Func<IBonModule, TContext, Task> phaseAction) 
        where TContext : class
    {
        foreach (var moduleInfo in Modules)
        {
            if (moduleInfo.Instance != null)
            {
                try
                {
                    await phaseAction(moduleInfo.Instance, context);
                }
                catch (Exception ex)
                {
                    // Log or handle specific module errors as needed
                    // For example: logger.LogError(ex, $"Error during module {moduleInfo.Instance.GetType().Name} execution.");
                    throw new InvalidOperationException(
                        $"An error occurred while executing a module phase for {moduleInfo.Instance.GetType().Name}.", ex);
                }
            }
        }
    }
}