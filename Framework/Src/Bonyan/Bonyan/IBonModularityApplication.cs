using Bonyan.DependencyInjection;
using Bonyan.Modularity.Abstractions;
using Bonyan.Reflection;

namespace Bonyan;

public interface IBonModularityApplication : IBonModuleConfigurator, IBonModuleInitializer, IBonServiceProviderAccessor,IBonModuleContainer
{
}