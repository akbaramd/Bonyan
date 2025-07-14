using Microsoft.Extensions.Localization;

namespace Bonyan.Localization;

public static class BonEnumLocalizerExtensions
{
    public static string GetString<TEnum>(this IBonEnumLocalizer iBonEnumLocalizer, object enumValue)
        where TEnum : Enum
    {
        return iBonEnumLocalizer.GetString(typeof(TEnum), enumValue);
    }

    public static string GetString<TEnum>(this IBonEnumLocalizer iBonEnumLocalizer, object enumValue, IStringLocalizer[] specifyLocalizers)
        where TEnum : Enum
    {
        return iBonEnumLocalizer.GetString(typeof(TEnum), enumValue, specifyLocalizers);
    }
}
