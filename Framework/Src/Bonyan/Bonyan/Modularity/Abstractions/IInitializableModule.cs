namespace Bonyan.Modularity.Abstractions;

public interface IInitializableModule
{
    Task OnPreInitializeAsync(BonInitializedContext modularityContext);
    Task OnInitializeAsync(BonInitializedContext modularityContext);
    Task OnPostInitializeAsync(BonInitializedContext modularityContext);
}
