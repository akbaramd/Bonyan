using System.Text.Json;
using Bonyan.Exceptions;
using Microsoft.Extensions.Localization;

namespace Bonyan.Localization.Json;

public static class JsonLocalizationDictionaryBuilder
{
    /// <summary>
    ///     Builds an <see cref="JsonLocalizationDictionaryBuilder" /> from given file.
    /// </summary>
    /// <param name="filePath">Path of the file</param>
    public static ILocalizationDictionary? BuildFromFile(string filePath)
    {
        try
        {
            return BuildFromJsonString(File.ReadAllText(filePath));
        }
        catch (Exception ex)
        {
            throw new BonException("Invalid localization file format: " + filePath, ex);
        }
    }

    private static readonly JsonSerializerOptions DeserializeOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    /// <summary>
    ///     Builds an <see cref="JsonLocalizationDictionaryBuilder" /> from given json string.
    /// </summary>
    /// <param name="jsonString">Json string</param>
    public static ILocalizationDictionary? BuildFromJsonString(string jsonString)
    {
        JsonLocalizationFile? jsonFile;
        try
        {
            jsonFile = JsonSerializer.Deserialize<JsonLocalizationFile>(jsonString, DeserializeOptions);
        }
        catch (JsonException ex)
        {
            throw new BonException("Can not parse json string. " + ex.Message);
        }

        if (jsonFile == null)
        {
            return null;
        }
        
        var cultureCode = jsonFile.Culture;
        if (string.IsNullOrEmpty(cultureCode))
        {
            return null;
        }

        var dictionary = new Dictionary<string, LocalizedString>();
        var dublicateNames = new List<string>();
        foreach (var item in jsonFile.Texts)
        {
            if (string.IsNullOrEmpty(item.Key))
            {
                throw new BonException("The key is empty in given json string.");
            }

            if (dictionary.GetOrDefault(item.Key) != null)
            {
                dublicateNames.Add(item.Key);
            }

            dictionary[item.Key] = new LocalizedString(item.Key, item.Value.NormalizeLineEndings());
        }

        if (dublicateNames.Count > 0)
        {
            throw new BonException(
                "A dictionary can not contain same key twice. There are some duplicated names: " +
                dublicateNames.JoinAsString(", "));
        }

        return new StaticLocalizationDictionary(cultureCode, dictionary);
    }
}
