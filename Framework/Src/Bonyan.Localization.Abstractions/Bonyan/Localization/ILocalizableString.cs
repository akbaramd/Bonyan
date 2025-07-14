using Microsoft.Extensions.Localization;

namespace Bonyan.Localization;

public interface ILocalizableString
{
    LocalizedString Localize(IStringLocalizerFactory stringLocalizerFactory);
}