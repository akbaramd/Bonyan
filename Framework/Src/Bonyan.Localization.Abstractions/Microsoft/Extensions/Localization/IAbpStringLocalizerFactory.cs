using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.Localization;


public interface IBonStringLocalizerFactory
{
    IStringLocalizer? CreateDefaultOrNull();

    IStringLocalizer? CreateByResourceNameOrNull([NotNull] string resourceName);
    
    Task<IStringLocalizer?> CreateByResourceNameOrNullAsync([NotNull] string resourceName);
}