namespace Bonyan.Modularity.Abstractions;

public interface IBonModuleConfigurator
{
    Task ConfigureModulesAsync(Action<BonConfigurationContext> configure);
}
