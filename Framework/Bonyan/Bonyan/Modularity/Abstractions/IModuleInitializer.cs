namespace Bonyan.Modularity.Abstractions;

public interface IBonModuleInitializer
{
    Task InitializeModulesAsync(IServiceProvider serviceProvider);
}
