using Microsoft.Extensions.Localization;

namespace Bonyan.Localization;

public interface IBonStringLocalizer : IStringLocalizer
{
    IEnumerable<LocalizedString> GetAllStrings(
        bool includeParentCultures,
        bool includeBaseLocalizers,
        bool includeDynamicContributors
    );

    Task<IEnumerable<LocalizedString>> GetAllStringsAsync(
        bool includeParentCultures
    );
    
    Task<IEnumerable<LocalizedString>> GetAllStringsAsync(
        bool includeParentCultures,
        bool includeBaseLocalizers,
        bool includeDynamicContributors
    );

    Task<IEnumerable<string>> GetSupportedCulturesAsync();
}