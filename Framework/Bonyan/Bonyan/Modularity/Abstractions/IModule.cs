using Autofac.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Modularity.Abstractions;

public interface IModule : global::Autofac.Core.IModule,IConfigurableModule, IInitializableModule
{
    public IServiceCollection Services { get; set; }
    public List<Type> DependedModules { get; set; }


    void DependOn<TModule>() where TModule : IModule;
    void DependOn(params Type[] type);
}
