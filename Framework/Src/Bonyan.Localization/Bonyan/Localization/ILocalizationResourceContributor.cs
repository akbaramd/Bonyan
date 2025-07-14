using Microsoft.Extensions.Localization;

namespace Bonyan.Localization;

public interface ILocalizationResourceContributor
{
    bool IsDynamic { get; }
    
    void Initialize(LocalizationResourceInitializationContext context);

    LocalizedString? GetOrNull(string cultureName, string name);

    void Fill(string cultureName, Dictionary<string, LocalizedString> dictionary);

    Task FillAsync(string cultureName, Dictionary<string, LocalizedString> dictionary);

    Task<IEnumerable<string>> GetSupportedCulturesAsync();
}
