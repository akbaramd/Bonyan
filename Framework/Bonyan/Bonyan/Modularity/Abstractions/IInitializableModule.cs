namespace Bonyan.Modularity.Abstractions;

public interface IInitializableModule
{
    Task OnPreInitializeAsync(ModularityInitializedContext modularityContext);
    Task OnInitializeAsync(ModularityInitializedContext modularityContext);
    Task OnPostInitializeAsync(ModularityInitializedContext modularityContext);
}
