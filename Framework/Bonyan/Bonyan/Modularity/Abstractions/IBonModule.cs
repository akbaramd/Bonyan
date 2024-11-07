using Autofac.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Modularity.Abstractions;

public interface IBonModule : global::Autofac.Core.IModule,IConfigurableModule, IInitializableModule
{
    public IServiceCollection Services { get; set; }
    public List<Type> DependedModules { get; set; }


    void DependOn<TModule>() where TModule : IBonModule;
    void DependOn(params Type[] type);
}
