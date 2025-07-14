using Microsoft.Extensions.Options;

namespace Bonyan.Localization;

public class DefaultLanguageProvider : ILanguageProvider
{
    protected BonLocalizationOptions Options { get; }

    public DefaultLanguageProvider(IOptions<BonLocalizationOptions> options)
    {
        Options = options.Value;
    }

    public Task<IReadOnlyList<LanguageInfo>> GetLanguagesAsync()
    {
        return Task.FromResult((IReadOnlyList<LanguageInfo>)Options.Languages);
    }
}
