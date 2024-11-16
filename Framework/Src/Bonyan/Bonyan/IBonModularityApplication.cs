using Bonyan.DependencyInjection;
using Bonyan.Modularity.Abstractions;

namespace Bonyan;

public interface IBonModularityApplication : IBonModuleConfigurator, IBonModuleInitializer, IBonServiceProviderAccessor
{
}