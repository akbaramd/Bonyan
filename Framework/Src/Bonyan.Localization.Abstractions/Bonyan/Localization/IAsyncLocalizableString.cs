using Microsoft.Extensions.Localization;

namespace Bonyan.Localization;

public interface IAsyncLocalizableString
{
    Task<LocalizedString> LocalizeAsync(IStringLocalizerFactory stringLocalizerFactory);
}