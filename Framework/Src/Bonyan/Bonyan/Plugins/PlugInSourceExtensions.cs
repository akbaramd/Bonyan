using Bonyan.Core;
using Bonyan.Modularity;

using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Bonyan.Plugins;

public static class PlugInSourceExtensions
{
    [NotNull]
    public static Type[] GetModulesWithAllDependencies([NotNull] this IPlugInSource plugInSource)
    {
        Check.NotNull(plugInSource, nameof(plugInSource));

        return plugInSource
            .GetModules()
            .SelectMany(type => BonyanModuleHelper.FindAllModuleTypes(type))
            .Distinct()
            .ToArray();
    }
}
