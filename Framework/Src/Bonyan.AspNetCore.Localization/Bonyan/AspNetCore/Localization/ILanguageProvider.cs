namespace Bonyan.AspNetCore.Localization;

public interface ILanguageProvider
{
    Task<IReadOnlyList<LanguageInfo>> GetLanguagesAsync();
}
