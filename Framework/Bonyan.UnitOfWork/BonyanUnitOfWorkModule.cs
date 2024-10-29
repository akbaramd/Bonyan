using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.UnitOfWork;
using Microsoft;
using Microsoft.Extensions.DependencyInjection;

public class BonyanUnitOfWorkModule : Module
{
  public override Task OnPreConfigureAsync(ServiceConfigurationContext context)
  {
    context.Services.AddTransient<UnitOfWorkInterceptor>();
    context.Services.OnRegistered(c=>c.Interceptors.TryAdd<UnitOfWorkInterceptor>());
    return base.OnPreConfigureAsync(context);
  }

  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {

    return base.OnConfigureAsync(context);
  }

}