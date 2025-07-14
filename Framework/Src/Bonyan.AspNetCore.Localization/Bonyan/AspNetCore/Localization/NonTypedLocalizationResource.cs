using JetBrains.Annotations;

namespace Bonyan.AspNetCore.Localization;

public class NonTypedLocalizationResource : LocalizationResourceBase
{
    public NonTypedLocalizationResource(
        [NotNull] string resourceName,
        string? defaultCultureName = null,
        ILocalizationResourceContributor? initialContributor = null
    ) : base(
        resourceName,
        defaultCultureName,
        initialContributor)
    {
    }
}