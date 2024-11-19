using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Bonyan.Plugins;


public class PlugInSourceList : List<IPlugInSource>
{
    [NotNull]
    internal Type[] GetAllModules()
    {
        return this
            .SelectMany(pluginSource => pluginSource.GetModulesWithAllDependencies())
            .Distinct()
            .ToArray();
    }
}
