using Microsoft.Extensions.Localization;

namespace Bonyan.Localization;

public class BonEnumLocalizer : IBonEnumLocalizer
{
    protected readonly IStringLocalizerFactory StringLocalizerFactory;

    public BonEnumLocalizer(IStringLocalizerFactory stringLocalizerFactory)
    {
        StringLocalizerFactory = stringLocalizerFactory;
    }

    public virtual string GetString(Type enumType, object enumValue)
    {
        return GetStringInternal(enumType, enumValue, StringLocalizerFactory.CreateDefaultOrNull());
    }

    public virtual string GetString(Type enumType, object enumValue, params IStringLocalizer?[] specifyLocalizers)
    {
        return GetStringInternal(enumType, enumValue, specifyLocalizers);
    }

    protected virtual string GetStringInternal(Type enumType, object enumValue, params IStringLocalizer?[] specifyLocalizers)
    {
        var memberName = enumType.GetEnumName(enumValue)!;
        var localizedString = GetStringOrNull(
            specifyLocalizers,
            new[]
            {
                $"Enum:{enumType.Name}.{enumValue}",
                $"Enum:{enumType.Name}.{memberName}",
                $"{enumType.Name}.{enumValue}",
                $"{enumType.Name}.{memberName}",
                memberName
            }
        );

        return localizedString ?? memberName;
    }

    protected virtual string? GetStringOrNull(IStringLocalizer?[] localizers, IEnumerable<string> keys)
    {
        foreach (var key in keys)
        {
            foreach (var l in localizers)
            {
                if (l == null)
                {
                    continue;
                }

                var localizedString = l[key];
                if (!localizedString.ResourceNotFound)
                {
                    return localizedString.Value;
                }
            }
        }

        return null;
    }
}
