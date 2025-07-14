using Microsoft.Extensions.Options;

namespace Bonyan.AspNetCore.Localization;

public class DefaultLanguageProvider : ILanguageProvider
{
    protected AbpLocalizationOptions Options { get; }

    public DefaultLanguageProvider(IOptions<AbpLocalizationOptions> options)
    {
        Options = options.Value;
    }

    public Task<IReadOnlyList<LanguageInfo>> GetLanguagesAsync()
    {
        return Task.FromResult((IReadOnlyList<LanguageInfo>)Options.Languages);
    }
}
