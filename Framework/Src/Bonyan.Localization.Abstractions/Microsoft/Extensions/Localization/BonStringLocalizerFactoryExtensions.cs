using System.Threading.Tasks;
using Bonyan.Exceptions;
using JetBrains.Annotations;

namespace Microsoft.Extensions.Localization;

public static class BonStringLocalizerFactoryExtensions
{
    public static IStringLocalizer? CreateDefaultOrNull(this IStringLocalizerFactory localizerFactory)
    {
        return (localizerFactory as IBonStringLocalizerFactory)
            ?.CreateDefaultOrNull();
    }

    public static IStringLocalizer? CreateByResourceNameOrNull(
        this IStringLocalizerFactory localizerFactory,
        string resourceName)
    {
        return (localizerFactory as IBonStringLocalizerFactory)
            ?.CreateByResourceNameOrNull(resourceName);
    }
    
    [NotNull]
    public static IStringLocalizer CreateByResourceName(
        this IStringLocalizerFactory localizerFactory,
        string resourceName)
    {
        var localizer = localizerFactory.CreateByResourceNameOrNull(resourceName);
        if (localizer == null)
        {
            throw new BonException("Couldn't find a localizer with given resource name: " + resourceName);
        }
        
        return localizer;
    }
    
    public static async Task<IStringLocalizer?> CreateByResourceNameOrNullAsync(
        this IStringLocalizerFactory localizerFactory,
        string resourceName)
    {
        var abpLocalizerFactory = localizerFactory as IStringLocalizerFactory;
        if (abpLocalizerFactory == null)
        {
            return null;
        } 
        
        return await abpLocalizerFactory.CreateByResourceNameOrNullAsync(resourceName);
    }
    
    [NotNull]
    public async static Task<IStringLocalizer> CreateByResourceNameAsync(
        this IStringLocalizerFactory localizerFactory,
        string resourceName)
    {
        var localizer = await localizerFactory.CreateByResourceNameOrNullAsync(resourceName);
        if (localizer == null)
        {
            throw new BonException("Couldn't find a localizer with given resource name: " + resourceName);
        }
        
        return localizer;
    }
}
