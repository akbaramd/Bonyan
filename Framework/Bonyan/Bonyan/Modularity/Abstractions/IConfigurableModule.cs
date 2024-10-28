namespace Bonyan.Modularity.Abstractions;

public interface IConfigurableModule
{
    Task OnPreConfigureAsync(ServiceConfigurationContext serviceConfigurationContext);
    Task OnConfigureAsync(ServiceConfigurationContext serviceConfigurationContext);
    Task OnPostConfigureAsync(ServiceConfigurationContext serviceConfigurationContext);
}
