﻿using JetBrains.Annotations;
using Microsoft.Extensions.Localization;

namespace Bonyan.Localization;

public static class HasNameWithLocalizableDisplayNameExtensions
{
    [NotNull]
    public static string GetLocalizedDisplayName(
        [NotNull] this IHasNameWithLocalizableDisplayName source,
        [NotNull] IStringLocalizerFactory stringLocalizerFactory,
        string? localizationNamePrefix = "DisplayName:")
    {
        if (source.DisplayName != null)
        {
            return source.DisplayName.Localize(stringLocalizerFactory);
        }

        var defaultStringLocalizer = stringLocalizerFactory.CreateDefaultOrNull();
        if (defaultStringLocalizer == null)
        {
            return source.Name;
        }

        var localizedString = defaultStringLocalizer[$"{localizationNamePrefix}{source.Name}"];
        if (!localizedString.ResourceNotFound ||
            localizationNamePrefix.IsNullOrEmpty())
        {
            return localizedString;
        }

        return defaultStringLocalizer[source.Name];
    }
}
