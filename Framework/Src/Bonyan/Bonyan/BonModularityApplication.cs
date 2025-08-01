using System.Linq;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Plugins;
using Bonyan.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bonyan;

/// <summary>
/// Manages the modular application lifecycle, including module loading, configuration, and initialization.
/// </summary>
/// <typeparam name="TModule">The type of the root module.</typeparam>
public class BonModularityApplication<TModule> : IBonModularityApplication where TModule : IBonModule
{
    private readonly IServiceCollection _serviceCollection;
    private readonly string _serviceName;
    private readonly IBonModuleLoader _moduleLoader;
    private readonly IAssemblyFinder _assemblyFinder;
    private readonly ITypeFinder _typeFinder;
    private readonly PlugInSourceList _plugInSources;
    private readonly Action<BonConfigurationContext>? _creationContext;

    /// <summary>
    /// Gets the service provider for dependency resolution.
    /// </summary>
    public IServiceProvider ServiceProvider { get; private set; }

    /// <summary>
    /// Gets all loaded modules.
    /// </summary>
    public IReadOnlyList<BonModuleDescriptor> Modules { get; }

    /// <summary>
    /// Gets the plugin sources used by the application.
    /// </summary>
    public PlugInSourceList PlugInSources => _plugInSources;

    /// <summary>
    /// Initializes a new instance of <see cref="BonModularityApplication{TModule}"/>.
    /// </summary>
    /// <param name="serviceCollection">The service collection for dependency registration.</param>
    /// <param name="serviceName">The service name for the application.</param>
    /// <param name="creationContext">Optional configuration during application creation.</param>
    public BonModularityApplication(
        IServiceCollection serviceCollection,
        string serviceName,
        Action<BonConfigurationContext>? creationContext = null)
    {
        _serviceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
        _serviceName = serviceName ?? throw new ArgumentNullException(nameof(serviceName));
        _creationContext = creationContext;

        // Initialize core services
        _moduleLoader = new BonModuleLoader();
        _assemblyFinder = new AssemblyFinder(this);
        _typeFinder = new TypeFinder(_assemblyFinder);
        _plugInSources = new PlugInSourceList();

        // Initialize plugin sources from creation context FIRST
        InitializePluginSources();

        // Load modules with properly populated plugin sources
        Modules = _moduleLoader.LoadModules(_serviceCollection, typeof(TModule), _plugInSources);
        
        ConfigureModulesAsync(_creationContext).GetAwaiter().GetResult();

        RegisterCoreServices();
    }

    /// <summary>
    /// Configures all modules in the application.
    /// </summary>
    public Task ConfigureModulesAsync(Action<BonConfigurationContext>? action = null)
    {
        // Register additional services and execute configuration phases
        _serviceCollection.AddBonyan(_serviceName, context =>
        {
            // Ensure plugin sources are available in the configuration context
            if (!context.PlugInSources.Any() && _plugInSources.Any())
            {
                context.PlugInSources.AddRange(_plugInSources);
            }
            
            action?.Invoke(context);
            
            InitializeModuleServices();

            ExecuteModulePhases(context, (module, ctx) => module.OnPreConfigureAsync(ctx)).GetAwaiter().GetResult();
            ExecuteModulePhases(context, (module, ctx) => module.OnConfigureAsync(ctx)).GetAwaiter().GetResult();
            ExecuteModulePhases(context, (module, ctx) => module.OnPostConfigureAsync(ctx)).GetAwaiter().GetResult();
        });

        // Build the service provider
        ServiceProvider = _serviceCollection.BuildServiceProvider();

        _serviceCollection.AddObjectAccessor(ServiceProvider);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Initializes all modules in the application.
    /// </summary>
    /// <param name="serviceProvider">The service provider to use during initialization.</param>
    public async Task InitializeModulesAsync(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        serviceProvider.InitializeBonyan(context =>
        {
            ExecuteModulePhases(context, (module, ctx) => module.OnPreInitializeAsync(ctx)).GetAwaiter().GetResult();
            ExecuteModulePhases(context, (module, ctx) => module.OnInitializeAsync(ctx)).GetAwaiter().GetResult();
            ExecuteModulePhases(context, (module, ctx) => module.OnPostInitializeAsync(ctx)).GetAwaiter().GetResult();
        });
    }

    /// <summary>
    /// Executes a specific lifecycle phase for each module.
    /// </summary>
    /// <typeparam name="TContext">The context type passed to the module phase.</typeparam>
    /// <param name="context">The lifecycle context for the phase.</param>
    /// <param name="phaseAction">The action to execute for each module during the phase.</param>
    private async Task ExecuteModulePhases<TContext>(
        TContext context,
        Func<IBonModule, TContext, Task> phaseAction) where TContext : class
    {
        foreach (var module in Modules)
        {
            if (module.Instance == null) continue;

            try
            {
                await phaseAction(module.Instance, context);
            }
            catch (Exception ex)
            {
                // Log or handle module execution errors
                throw new InvalidOperationException(
                    $"An error occurred during the execution of module phase for {module.Instance.GetType().Name}.", ex);
            }
        }
    }

    /// <summary>
    /// Registers core services required by the modular application.
    /// </summary>
    private void RegisterCoreServices()
    {
        _serviceCollection.AddObjectAccessor(_moduleLoader);
        _serviceCollection.AddObjectAccessor<IAssemblyFinder>(_assemblyFinder);
        _serviceCollection.AddObjectAccessor(_plugInSources);

        _serviceCollection.TryAddSingleton(_moduleLoader);
        _serviceCollection.TryAddSingleton<IBonModuleContainer>(this);
        _serviceCollection.TryAddSingleton<IBonModuleConfigurator>(this);
        _serviceCollection.TryAddSingleton<IBonModuleInitializer>(this);
        _serviceCollection.TryAddSingleton<IBonModularityApplication>(this);
        _serviceCollection.TryAddSingleton<IAssemblyFinder>(_assemblyFinder);
        _serviceCollection.TryAddSingleton<ITypeFinder>(_typeFinder);
        _serviceCollection.TryAddSingleton(_plugInSources);
    }

    /// <summary>
    /// Initializes module services with the current service collection.
    /// </summary>
    private void InitializeModuleServices()
    {
        foreach (var module in Modules)
        {
            if (module.Instance != null)
            {
                module.Instance.Services = _serviceCollection;
            }
        }
    }

    /// <summary>
    /// Initializes plugin sources from the creation context.
    /// </summary>
    private void InitializePluginSources()
    {
        if (_creationContext == null) return;

        // Create a temporary context to populate plugin sources
        var tempContext = new BonConfigurationContext(_serviceCollection);
        
        // Invoke the creation context to populate plugin sources
        _creationContext(tempContext);
        
        // Copy the populated plugin sources to our instance
        _plugInSources.AddRange(tempContext.PlugInSources);
    }
}
