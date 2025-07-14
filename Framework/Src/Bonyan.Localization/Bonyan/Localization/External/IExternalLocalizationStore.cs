using JetBrains.Annotations;

namespace Bonyan.Localization.External;

public interface IExternalLocalizationStore
{
    LocalizationResourceBase? GetResourceOrNull([NotNull] string resourceName);
    
    Task<LocalizationResourceBase?> GetResourceOrNullAsync([NotNull] string resourceName);
    
    Task<string[]> GetResourceNamesAsync();
    
    Task<LocalizationResourceBase[]> GetResourcesAsync();
}