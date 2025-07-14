namespace Bonyan.Localization
{
    public interface IBonLocalizationManager
    {
        string GetString(string name);
        string GetString(string name, params object[] arguments);
        string GetString(string name, string defaultValue);
        string GetString(string name, string defaultValue, params object[] arguments);
        string GetString(string name, string defaultValue, string culture);
        string GetString(string name, string defaultValue, string culture, params object[] arguments);
        string GetString<TResource>(string name);
        string GetString<TResource>(string name, params object[] arguments);
        string GetString<TResource>(string name, string defaultValue);
        string GetString<TResource>(string name, string defaultValue, params object[] arguments);
        string GetString<TResource>(string name, string defaultValue, string culture);
        string GetString<TResource>(string name, string defaultValue, string culture, params object[] arguments);
        LocalizationResourceInfo? GetResourceInfo<TResource>();
        LocalizationResourceInfo? GetResourceInfo(Type resourceType);
        string GetCurrentCulture();
        string GetCurrentUICulture();
        void SetCulture(string culture);
        void SetUICulture(string culture);
    }
} 