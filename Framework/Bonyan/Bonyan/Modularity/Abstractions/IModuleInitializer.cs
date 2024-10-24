namespace Bonyan.Modularity.Abstractions;

public interface IModuleInitializer
{
    Task InitializeModulesAsync(ModularityInitializedContext context);
}
