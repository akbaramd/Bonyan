using JetBrains.Annotations;

namespace Bonyan.Localization;

public static class LanguageInfoExtensions
{
    public static T? FindByCulture<T>(
        [NotNull] this IEnumerable<T> languages,
        [NotNull] string cultureName,
        string? uiCultureName = null)
    where T : class, ILanguageInfo
    {
        if (uiCultureName == null)
        {
            uiCultureName = cultureName;
        }

        var languageList = languages.ToList();

        return languageList.FirstOrDefault(l => l.CultureName == cultureName && l.UiCultureName == uiCultureName)
               ?? languageList.FirstOrDefault(l => l.CultureName == cultureName)
               ?? languageList.FirstOrDefault(l => l.UiCultureName == uiCultureName);
    }
}
