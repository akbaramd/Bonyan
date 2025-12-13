using Bonyan.Plugins;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Modularity.Abstractions;

public interface IBonModuleLoader
{
    BonModuleDescriptor[] LoadModules(
        [NotNull] IServiceCollection services,
        [NotNull] Type startupModuleType,
        [NotNull] string serviceKey,
        [NotNull] string serviceTitle,
        [NotNull] PlugInSourceList plugInSources
    );
}