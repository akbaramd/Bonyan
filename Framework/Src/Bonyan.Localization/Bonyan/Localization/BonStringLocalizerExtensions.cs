using System.Reflection;
using Bonyan.Core;
using Bonyan.DynamicProxy;
using Bonyan.Exceptions;
using Bonyan.Helpers;
using JetBrains.Annotations;
using Microsoft.Extensions.Localization;

namespace Bonyan.Localization;

public static class BonStringLocalizerExtensions
{
    [NotNull]
    public static IStringLocalizer GetInternalLocalizer(
        [NotNull] this IStringLocalizer stringLocalizer)
    {
        Check.NotNull(stringLocalizer, nameof(stringLocalizer));

        var localizerType = stringLocalizer.GetType();
        if (!ReflectionHelper.IsAssignableToGenericType(localizerType, typeof(StringLocalizer<>)))
        {
            return stringLocalizer;
        }

        var localizerField = localizerType
            .GetField(
                "_localizer",
                BindingFlags.Instance |
                BindingFlags.NonPublic
            );

        if (localizerField == null)
        {
            throw new BonException($"Could not find the _localizer field inside the {typeof(StringLocalizer<>).FullName} class. Probably its name has changed. Please report this issue to the ABP framework.");
        }

        return (localizerField.GetValue(stringLocalizer) as IStringLocalizer)!;
    }

    public static IEnumerable<LocalizedString> GetAllStrings(
        this IStringLocalizer stringLocalizer,
        bool includeParentCultures,
        bool includeBaseLocalizers,
        bool includeDynamicContributors)
    {
        var internalLocalizer = ((IStringLocalizer)ProxyHelper.UnProxy(stringLocalizer)).GetInternalLocalizer();
        if (internalLocalizer is IBonStringLocalizer abpStringLocalizer)
        {
            return abpStringLocalizer.GetAllStrings(
                includeParentCultures,
                includeBaseLocalizers,
                includeDynamicContributors
            );
        }

        return stringLocalizer.GetAllStrings(
            includeParentCultures
        );
    }
    
    public static async Task<IEnumerable<LocalizedString>> GetAllStringsAsync(
        this IStringLocalizer stringLocalizer,
        bool includeParentCultures,
        bool includeBaseLocalizers,
        bool includeDynamicContributors)
    {
        var internalLocalizer = ((IStringLocalizer)ProxyHelper.UnProxy(stringLocalizer)).GetInternalLocalizer();
        if (internalLocalizer is IBonStringLocalizer abpStringLocalizer)
        {
            return await abpStringLocalizer.GetAllStringsAsync(
                includeParentCultures,
                includeBaseLocalizers,
                includeDynamicContributors
            );
        }

        return stringLocalizer.GetAllStrings(
            includeParentCultures
        );
    }

    public static async Task<IEnumerable<string>> GetSupportedCulturesAsync(this IStringLocalizer localizer)
    {
        var internalLocalizer = ((IStringLocalizer)ProxyHelper.UnProxy(localizer)).GetInternalLocalizer();
        if (internalLocalizer is IBonStringLocalizer abpStringLocalizer)
        {
            return await abpStringLocalizer.GetSupportedCulturesAsync();
        }

        return Array.Empty<string>();
    }
    
    public static Task<IEnumerable<LocalizedString>> GetAllStringsAsync(
        this IStringLocalizer localizer)
    {
        return localizer.GetAllStringsAsync(includeParentCultures: true);
    }
    
    public static Task<IEnumerable<LocalizedString>> GetAllStringsAsync(
        this IStringLocalizer localizer,
        bool includeParentCultures)
    {
        Check.NotNull(localizer, nameof(localizer));
        
        var internalLocalizer = ((IStringLocalizer)ProxyHelper.UnProxy(localizer)).GetInternalLocalizer();
        if (internalLocalizer is IBonStringLocalizer abpStringLocalizer)
        {
            return abpStringLocalizer.GetAllStringsAsync(includeParentCultures: includeParentCultures);
        }

        return Task.FromResult(
            localizer.GetAllStrings(includeParentCultures: true)
        );
    }
}
