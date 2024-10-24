namespace Bonyan.Modularity.Abstractions;

public interface IModuleConfigurator
{
    Task ConfigureModulesAsync(ModularityContext context);
}
