using System.Globalization;
using Bonyan.Core;

namespace Bonyan.Localization;

public static class BonLocalizationOptionsExtensions
{
    public static BonLocalizationOptions AddLanguagesMapOrUpdate(this BonLocalizationOptions localizationOptions,
        string packageName, params NameValue[] maps)
    {
        foreach (var map in maps)
        {
            AddOrUpdate(localizationOptions.LanguagesMap, packageName, map);
        }

        return localizationOptions;
    }

    public static string GetLanguagesMap(this BonLocalizationOptions localizationOptions, string packageName,
        string language)
    {
        return localizationOptions.LanguagesMap.TryGetValue(packageName, out var maps)
            ? maps.FirstOrDefault(x => x.Name == language)?.Value ?? language
            : language;
    }

    public static string GetCurrentUICultureLanguagesMap(this BonLocalizationOptions localizationOptions, string packageName)
    {
        return GetLanguagesMap(localizationOptions, packageName, CultureInfo.CurrentUICulture.Name);
    }

    public static BonLocalizationOptions AddLanguageFilesMapOrUpdate(this BonLocalizationOptions localizationOptions,
        string packageName, params NameValue[] maps)
    {
        foreach (var map in maps)
        {
            AddOrUpdate(localizationOptions.LanguageFilesMap, packageName, map);
        }

        return localizationOptions;
    }

    public static string GetLanguageFilesMap(this BonLocalizationOptions localizationOptions, string packageName,
        string language)
    {
        return localizationOptions.LanguageFilesMap.TryGetValue(packageName, out var maps)
            ? maps.FirstOrDefault(x => x.Name == language)?.Value ?? language
            : language;
    }

    public static string GetCurrentUICultureLanguageFilesMap(this BonLocalizationOptions localizationOptions, string packageName)
    {
        return GetLanguageFilesMap(localizationOptions, packageName, CultureInfo.CurrentUICulture.Name);
    }

    private static void AddOrUpdate(IDictionary<string, List<NameValue>> maps, string packageName, NameValue value)
    {
        if (maps.TryGetValue(packageName, out var existMaps))
        {
            existMaps.GetOrAdd(x => x.Name == value.Name, () => value).Value = value.Value;
        }
        else
        {
            maps.Add(packageName, new List<NameValue> { value });
        }
    }
}
