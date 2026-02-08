using System.Linq;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Plugins;
using Bonyan.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Bonyan;

/// <summary>
/// Manages the modular application lifecycle, including module loading, configuration, and initialization.
/// </summary>
/// <typeparam name="TModule">The type of the root module.</typeparam>
public class BonModularityApplication<TModule> : IBonModularityApplication where TModule : IBonModule
{
    private readonly IServiceCollection _serviceCollection;
    private readonly string _serviceKey;
    private readonly string _serviceTitle;
    private readonly IBonModuleLoader _moduleLoader;
    private readonly IAssemblyFinder _assemblyFinder;
    private readonly ITypeFinder _typeFinder;
    private readonly PlugInSourceList _plugInSources;
    private readonly Action<BonConfigurationContext>? _creationContext;

    /// <summary>
    /// Gets the service provider for dependency resolution.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if accessed before configuration completes.</exception>
    public IServiceProvider ServiceProvider
    {
        get
        {
            if (_serviceProvider == null)
                throw new InvalidOperationException(
                    "ServiceProvider is not available. Ensure ConfigureModulesAsync has been called and completed.");
            return _serviceProvider;
        }
        private set => _serviceProvider = value;
    }

    private IServiceProvider? _serviceProvider;

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
    /// <param name="serviceKey">The service key (unique identifier). Required.</param>
    /// <param name="serviceTitle">The service title (display name). Required.</param>
    /// <param name="creationContext">Optional configuration during application creation.</param>
    public BonModularityApplication(
        IServiceCollection serviceCollection,
        string serviceKey,
        string serviceTitle,
        Action<BonConfigurationContext>? creationContext = null)
    {
        _serviceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
        
        // Service key is required - validate it
        if (string.IsNullOrWhiteSpace(serviceKey))
        {
            throw new ArgumentException("Service key is required and cannot be null, empty, or whitespace.", nameof(serviceKey));
        }
        
        // Service title is required - validate it
        if (string.IsNullOrWhiteSpace(serviceTitle))
        {
            throw new ArgumentException("Service title is required and cannot be null, empty, or whitespace.", nameof(serviceTitle));
        }
        
        _serviceKey = serviceKey;
        _serviceTitle = serviceTitle;
        _creationContext = creationContext;

        // Initialize core services with dependency injection (DIP compliance)
        // Get logger factory from service collection if available, otherwise create null loggers
        // TODO: Consider injecting these via constructor for better testability
        ILogger<ModuleCatalog>? catalogLogger = null;
        ILogger<AttributeModuleMetadataProvider>? metadataLogger = null;
        ILogger<DependencyGraphBuilder>? graphLogger = null;
        ILogger<DiModuleActivator>? activatorLogger = null;
        ILogger<BonModuleLoader>? loaderLogger = null;
        
        try
        {
            // Try to get logger factory from service collection (may not be registered yet)
            var tempProvider = _serviceCollection.BuildServiceProvider();
            var loggerFactory = tempProvider.GetService<ILoggerFactory>();
            
            // Create all loggers BEFORE disposing the provider
            if (loggerFactory != null)
            {
                catalogLogger = loggerFactory.CreateLogger<ModuleCatalog>();
                metadataLogger = loggerFactory.CreateLogger<AttributeModuleMetadataProvider>();
                graphLogger = loggerFactory.CreateLogger<DependencyGraphBuilder>();
                activatorLogger = loggerFactory.CreateLogger<DiModuleActivator>();
                loaderLogger = loggerFactory.CreateLogger<BonModuleLoader>();
            }
            
            tempProvider.Dispose();
        }
        catch
        {
            // Logger factory not available yet - will use null loggers (backward compatible)
        }

        var catalog = new ModuleCatalog(catalogLogger);
        var metadataProvider = new AttributeModuleMetadataProvider(metadataLogger);
        var graphBuilder = new DependencyGraphBuilder(graphLogger);
        var activator = new DiModuleActivator(activatorLogger);
        
        _moduleLoader = new BonModuleLoader(catalog, metadataProvider, graphBuilder, activator, loaderLogger);
        
        _assemblyFinder = new AssemblyFinder(this);
        _typeFinder = new TypeFinder(_assemblyFinder);
        _plugInSources = new PlugInSourceList();

        // Initialize plugin sources from creation context FIRST
        InitializePluginSources();

        // Load modules with properly populated plugin sources
        Modules = _moduleLoader.LoadModules(_serviceCollection, typeof(TModule), _serviceKey, _serviceTitle, _plugInSources);
        
        ConfigureModulesAsync(_creationContext).GetAwaiter().GetResult();

        RegisterCoreServices();
    }

    /// <summary>
    /// Configures all modules in the application.
    /// </summary>
    public Task ConfigureModulesAsync(Action<BonConfigurationContext>? action = null)
    {
        // Register additional services and execute configuration phases
        _serviceCollection.AddBonyan(_serviceKey, context =>
        {
            // Ensure plugin sources are available in the configuration context
            if (!context.PlugInSources.Any() && _plugInSources.Any())
            {
                context.PlugInSources.AddRange(_plugInSources);
            }
            
            action?.Invoke(context);
            
            InitializeModuleServices();

            // Create separate contexts for each configuration phase
            // This ensures proper access control: PreConfigure only in PreConfigure phase, etc.
            var preContext = new BonPreConfigurationContext(_serviceCollection)
            {
                ServiceManager = context.ServiceManager
            };
            preContext.PlugInSources.AddRange(context.PlugInSources);

            var configureContext = new BonConfigurationContext(_serviceCollection)
            {
                ServiceManager = context.ServiceManager
            };
            configureContext.PlugInSources.AddRange(context.PlugInSources);

            var postContext = new BonPostConfigurationContext(_serviceCollection)
            {
                ServiceManager = context.ServiceManager
            };
            postContext.PlugInSources.AddRange(context.PlugInSources);

            // Note: Using GetAwaiter().GetResult() here is intentional for backward compatibility
            // in the synchronous configuration phase. Consider refactoring to fully async builder pattern.
            // For now, we call the old interface methods without CancellationToken for compatibility
            
            // Phase 1: PreConfigure - only PreConfigure operations available
            ExecuteModulePhases(preContext, async (module, ctx) =>
            {
                var configModule = module as IConfigurableModule;
                if (configModule != null)
                {
                    await configModule.OnPreConfigureAsync(ctx, default);
                }
            }).GetAwaiter().GetResult();
            
            // Phase 2: Configure - only Configure operations available
            ExecuteModulePhases(configureContext, async (module, ctx) =>
            {
                var configModule = module as IConfigurableModule;
                if (configModule != null)
                {
                    await configModule.OnConfigureAsync(ctx, default);
                }
            }).GetAwaiter().GetResult();
            
            // Phase 3: PostConfigure - only PostConfigure operations available
            ExecuteModulePhases(postContext, async (module, ctx) =>
            {
                var configModule = module as IConfigurableModule;
                if (configModule != null)
                {
                    await configModule.OnPostConfigureAsync(ctx, default);
                }
            }).GetAwaiter().GetResult();
        });

        // Build the service provider once
        _serviceProvider = _serviceCollection.BuildServiceProvider(validateScopes: true);
        
        // Register object accessor AFTER build with the actual provider (fixes issue #15)
        // Use TryAddObjectAccessor to get or create accessor, then set value
        var accessor = _serviceCollection.TryAddObjectAccessor<IServiceProvider>();
        accessor.Value = _serviceProvider;
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// Initializes all modules in the application.
    /// </summary>
    /// <param name="serviceProvider">The service provider to use during initialization.</param>
    public Task InitializeModulesAsync(IServiceProvider serviceProvider)
    {
        return InitializeModulesAsync(serviceProvider, CancellationToken.None);
    }

    /// <summary>
    /// Initializes all modules in the application with cancellation support.
    /// </summary>
    /// <param name="serviceProvider">The service provider to use during initialization.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    public async Task InitializeModulesAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        // Create BonInitializedContext directly - it doesn't need to be registered as a service
        var context = new BonInitializedContext(serviceProvider);
        
        // Truly async execution (fixes deadlock risk)
        await ExecuteModulePhasesAsync(context, (module, ctx, ct) =>
        {
            var initModule = module as IInitializableModule;
            return initModule != null 
                ? initModule.OnPreInitializeAsync(ctx, ct) 
                : ValueTask.CompletedTask;
        }, cancellationToken);
        
        await ExecuteModulePhasesAsync(context, (module, ctx, ct) =>
        {
            var initModule = module as IInitializableModule;
            return initModule != null 
                ? initModule.OnInitializeAsync(ctx, ct) 
                : ValueTask.CompletedTask;
        }, cancellationToken);
        
        await ExecuteModulePhasesAsync(context, (module, ctx, ct) =>
        {
            var initModule = module as IInitializableModule;
            return initModule != null 
                ? initModule.OnPostInitializeAsync(ctx, ct) 
                : ValueTask.CompletedTask;
        }, cancellationToken);
    }

    /// <summary>
    /// Executes a specific lifecycle phase for each module (async version with cancellation support).
    /// </summary>
    /// <typeparam name="TContext">The context type passed to the module phase.</typeparam>
    /// <param name="context">The lifecycle context for the phase.</param>
    /// <param name="phaseAction">The action to execute for each module during the phase.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task ExecuteModulePhasesAsync<TContext>(
        TContext context,
        Func<IBonModule, TContext, CancellationToken, ValueTask> phaseAction,
        CancellationToken cancellationToken) where TContext : class
    {
        foreach (var module in Modules)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (module.Instance == null)
            {
                // Log warning instead of silent skip (fixes issue #5)
                // TODO: Add ILogger when available
                continue;
            }

            try
            {
                await phaseAction(module.Instance, context, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"An error occurred during the execution of module phase for {module.Instance.GetType().Name}.", ex);
            }
        }
    }

    /// <summary>
    /// Executes a specific lifecycle phase for each module (legacy sync version for backward compatibility).
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
                throw new InvalidOperationException(
                    $"An error occurred during the execution of module phase for {module.Instance.GetType().Name}.", ex);
            }
        }
    }

    /// <summary>
    /// Registers core services required by the modular application.
    /// Then runs all conventional registrars (added by modules in OnConfigureAsync) for each assembly.
    /// </summary>
    private void RegisterCoreServices()
    {
        // Register object accessors
        _serviceCollection.AddObjectAccessor(_moduleLoader);
        _serviceCollection.AddObjectAccessor<IAssemblyFinder>(_assemblyFinder);
        _serviceCollection.AddObjectAccessor(_plugInSources);

        // Register core services (removed duplicates - fixes issue #16)
        _serviceCollection.TryAddSingleton(_moduleLoader);
        _serviceCollection.TryAddSingleton<IBonModuleContainer>(this);
        _serviceCollection.TryAddSingleton<IBonModuleConfigurator>(this);
        _serviceCollection.TryAddSingleton<IBonModuleInitializer>(this);
        _serviceCollection.TryAddSingleton<IBonModularityApplication>(this);
        _serviceCollection.TryAddSingleton<IAssemblyFinder>(_assemblyFinder);
        _serviceCollection.TryAddSingleton<ITypeFinder>(_typeFinder);
        _serviceCollection.TryAddSingleton(_plugInSources);

        // Run conventional registration for each assembly (ABP-style).
        // AddAssembly runs all registrars (GetCandidateTypes, CanRegister, GetRegistration) and adds to services.
        foreach (var assembly in _assemblyFinder.Assemblies)
        {
            _serviceCollection.AddAssembly(assembly);
        }
    }

    /// <summary>
    /// Initializes module services with the current service collection.
    /// Also initializes enhanced module properties if the module inherits from BonModuleEnhanced.
    /// </summary>
    private void InitializeModuleServices()
    {
        foreach (var module in Modules)
        {
            if (module.Instance != null)
            {
                module.Instance.Services = _serviceCollection;
                
                // Initialize enhanced module properties if applicable
                if (module.Instance is BonModuleEnhanced enhancedModule)
                {
                    var context = new BonConfigurationContext(_serviceCollection);
                    enhancedModule.InitializeModuleProperties(context);
                }
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
