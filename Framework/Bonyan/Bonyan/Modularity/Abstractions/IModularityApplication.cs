namespace Bonyan.Modularity.Abstractions;

public interface IModularityApplication : IModuleConfigurator,IModuleInitializer
{
  public IServiceProvider ServiceProvider { get; set; }
}