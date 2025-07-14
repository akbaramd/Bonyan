using Microsoft.Extensions.Localization;

namespace Bonyan.AspNetCore.Localization;

public interface IAsyncLocalizableString
{
    Task<LocalizedString> LocalizeAsync(IStringLocalizerFactory stringLocalizerFactory);
}