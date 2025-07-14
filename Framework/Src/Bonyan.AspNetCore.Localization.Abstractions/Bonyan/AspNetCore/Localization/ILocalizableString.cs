using Microsoft.Extensions.Localization;

namespace Bonyan.AspNetCore.Localization;

public interface ILocalizableString
{
    LocalizedString Localize(IStringLocalizerFactory stringLocalizerFactory);
}