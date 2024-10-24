namespace Bonyan.Modularity.Abstractions;

public interface IConfigurableModule
{
    Task OnPreConfigureAsync(ModularityContext modularityContext);
    Task OnConfigureAsync(ModularityContext modularityContext);
    Task OnPostConfigureAsync(ModularityContext modularityContext);
}
