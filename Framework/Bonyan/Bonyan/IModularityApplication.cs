using Bonyan.DependencyInjection;
using Bonyan.Modularity.Abstractions;

namespace Bonyan;

public interface IModularityApplication : IModuleConfigurator, IModuleInitializer, IServiceProviderAccessor
{
}