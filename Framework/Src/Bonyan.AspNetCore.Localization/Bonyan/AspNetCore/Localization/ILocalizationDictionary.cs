using Microsoft.Extensions.Localization;

namespace Bonyan.AspNetCore.Localization;

/// <summary>
/// Represents a dictionary that is used to find a localized string.
/// </summary>
public interface ILocalizationDictionary
{
    string CultureName { get; }

    LocalizedString? GetOrNull(string name);

    void Fill(Dictionary<string, LocalizedString> dictionary);
}
