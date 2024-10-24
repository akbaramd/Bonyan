using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Modularity;

public interface IModularityApplication
{
  public IEnumerable<ModuleInfo> Modules { get;  }
  public Task ConfigureServiceAsync();
  public Task InitializeAsync();
}

public class ModularityApplication<TModule> : IModularityApplication where TModule : IModule
{
  private readonly IServiceCollection _serviceCollection;
  private IModuleManager _moduleManager;

  public ModularityApplication(IServiceCollection serviceCollection)
  {
    _serviceCollection = serviceCollection;
   

    _moduleManager = new ModuleManager();

    _moduleManager.LoadModules(typeof(TModule));
    serviceCollection.AddSingleton<IModuleManager>(_moduleManager);
  }

  public IEnumerable<ModuleInfo> Modules => _moduleManager.GetLoadedModules();

  public Task ConfigureServiceAsync()
  {
    var context = new ModularityContext(_serviceCollection,
      _serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>());
    return _moduleManager.ConfigureModulesAsync(context);
  }

  public Task InitializeAsync()
  {
    var buildServiceProvider = _serviceCollection.BuildServiceProvider();
    var context = new ModularityInitializedContext(buildServiceProvider,
      buildServiceProvider.GetRequiredService<IConfiguration>());
    return _moduleManager.InitializeModulesAsync(context);
  }
}
