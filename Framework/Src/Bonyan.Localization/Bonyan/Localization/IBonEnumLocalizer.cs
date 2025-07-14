using Microsoft.Extensions.Localization;

namespace Bonyan.Localization;

public interface IBonEnumLocalizer
{
    string GetString(Type enumType, object enumValue);

    string GetString(Type enumType, object enumValue, IStringLocalizer?[] specifyLocalizers);
}
