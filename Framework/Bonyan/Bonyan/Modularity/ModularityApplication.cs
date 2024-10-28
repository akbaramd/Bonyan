using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Modularity;

public class ModularityApplication<TModule> : IModularityApplication where TModule : IModule
{
    private readonly IServiceCollection _serviceCollection;
    private readonly IModuleAccessor _moduleAccessor;
    private readonly IModuleManager _moduleManager;

    public ModularityApplication(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
        _moduleAccessor = new ModuleAccessor();
        _moduleManager = new ModuleManager(_moduleAccessor);
        
        // Load and arrange modules
        _moduleManager.LoadModules(typeof(TModule));

        // Register core services
        _serviceCollection.AddSingleton(_moduleAccessor);
        _serviceCollection.AddSingleton(_moduleManager);
        _serviceCollection.AddSingleton<IModuleLoader>(_moduleManager);
        _serviceCollection.AddSingleton<IModuleConfigurator>(this);
        _serviceCollection.AddSingleton<IModuleInitializer>(this);
        
        // Configure services for modules
        ConfigureModulesAsync().GetAwaiter().GetResult();
    }

    public IEnumerable<ModuleInfo> Modules => _moduleManager.GetLoadedModules();

    public async Task ConfigureModulesAsync()
    {
        // Create ModularityContext
        var serviceProvider = _serviceCollection.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var context = new ServiceConfigurationContext(_serviceCollection, configuration);

        // Phase 1: Execute OnPreConfigureAsync for all modules
        foreach (var moduleInfo in Modules)
        {
            if (moduleInfo.Instance != null)
            {
                await moduleInfo.Instance.OnPreConfigureAsync(context);
            }
        }

        // Phase 2: Execute OnConfigureAsync for all modules
        foreach (var moduleInfo in Modules)
        {
            if (moduleInfo.Instance != null)
            {
                await moduleInfo.Instance.OnConfigureAsync(context);
            }
        }

        // Phase 3: Execute OnPostConfigureAsync for all modules
        foreach (var moduleInfo in Modules)
        {
            if (moduleInfo.Instance != null)
            {
                await moduleInfo.Instance.OnPostConfigureAsync(context);
            }
        }
    }

    public async Task InitializeModulesAsync(IServiceProvider serviceProvider)
    {
        // Create ModularityInitializedContext
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var context = new ServiceInitializationContext(serviceProvider, configuration);

        // Phase 1: Execute OnPreInitializeAsync for all modules
        foreach (var moduleInfo in Modules)
        {
            if (moduleInfo.Instance != null)
            {
                await moduleInfo.Instance.OnPreInitializeAsync(context);
            }
        }

        // Phase 2: Execute OnInitializeAsync for all modules
        foreach (var moduleInfo in Modules)
        {
            if (moduleInfo.Instance != null)
            {
                await moduleInfo.Instance.OnInitializeAsync(context);
            }
        }

        // Phase 3: Execute OnPostInitializeAsync for all modules
        foreach (var moduleInfo in Modules)
        {
            if (moduleInfo.Instance != null)
            {
                await moduleInfo.Instance.OnPostInitializeAsync(context);
            }
        }
    }
}
