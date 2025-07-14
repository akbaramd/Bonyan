namespace Bonyan.Localization;

public interface ILanguageProvider
{
    Task<IReadOnlyList<LanguageInfo>> GetLanguagesAsync();
}
