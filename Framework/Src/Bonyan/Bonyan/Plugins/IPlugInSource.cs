using JetBrains.Annotations;

namespace Bonyan.Plugins;

public interface IPlugInSource
{
    [NotNull]
    Type[] GetModules();
}
