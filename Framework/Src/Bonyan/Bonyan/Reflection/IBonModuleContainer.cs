using Bonyan.Modularity;
using JetBrains.Annotations;

namespace Bonyan.Reflection;

public interface IBonModuleContainer
{
    [NotNull]
    IReadOnlyList<BonModuleDescriptor> Modules { get; }
}
