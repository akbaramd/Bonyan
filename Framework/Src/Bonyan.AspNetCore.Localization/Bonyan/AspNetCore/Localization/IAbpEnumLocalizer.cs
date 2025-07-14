using Microsoft.Extensions.Localization;

namespace Bonyan.AspNetCore.Localization;

public interface IAbpEnumLocalizer
{
    string GetString(Type enumType, object enumValue);

    string GetString(Type enumType, object enumValue, IStringLocalizer?[] specifyLocalizers);
}
