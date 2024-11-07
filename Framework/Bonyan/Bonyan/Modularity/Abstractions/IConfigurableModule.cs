namespace Bonyan.Modularity.Abstractions;

public interface IConfigurableModule
{
    Task OnPreConfigureAsync(BonConfigurationContext bonConfigurationContext);
    Task OnConfigureAsync(BonConfigurationContext bonConfigurationContext);
    Task OnPostConfigureAsync(BonConfigurationContext bonConfigurationContext);
}
