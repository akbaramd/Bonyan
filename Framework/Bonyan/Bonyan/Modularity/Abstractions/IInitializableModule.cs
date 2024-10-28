namespace Bonyan.Modularity.Abstractions;

public interface IInitializableModule
{
    Task OnPreInitializeAsync(ServiceInitializationContext modularityContext);
    Task OnInitializeAsync(ServiceInitializationContext modularityContext);
    Task OnPostInitializeAsync(ServiceInitializationContext modularityContext);
}
