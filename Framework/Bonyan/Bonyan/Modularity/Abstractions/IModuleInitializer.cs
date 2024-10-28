namespace Bonyan.Modularity.Abstractions;

public interface IModuleInitializer
{
    Task InitializeModulesAsync(IServiceProvider serviceProvider);
}
