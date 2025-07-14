namespace Bonyan.Localization
{
    public interface IBonLocalizationService
    {
        string L(string name);
        string L(string name, params object[] arguments);
        string L(string name, string defaultValue);
        string L(string name, string defaultValue, params object[] arguments);
        string L(string name, string defaultValue, string culture);
        string L(string name, string defaultValue, string culture, params object[] arguments);
        string L<TResource>(string name);
        string L<TResource>(string name, params object[] arguments);
        string L<TResource>(string name, string defaultValue);
        string L<TResource>(string name, string defaultValue, params object[] arguments);
        string L<TResource>(string name, string defaultValue, string culture);
        string L<TResource>(string name, string defaultValue, string culture, params object[] arguments);
        string GetCurrentCulture();
        string GetCurrentUICulture();
        void SetCulture(string culture);
        void SetUICulture(string culture);
        LocalizationResourceInfo? GetResourceInfo<TResource>();
        LocalizationResourceInfo? GetResourceInfo(Type resourceType);
    }
} 