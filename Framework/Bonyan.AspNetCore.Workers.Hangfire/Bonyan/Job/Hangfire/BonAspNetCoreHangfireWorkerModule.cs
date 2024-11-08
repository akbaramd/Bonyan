using Autofac;
using Bonyan.AspNetCore.Job;
using Bonyan.Modularity;
using Hangfire;
using Microsoft;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bonyan.Job.Hangfire;


public class BonAspNetCoreWorkersHangfireModule : BonWebModule
{
  public BonAspNetCoreWorkersHangfireModule()
  {
    DependOn<BonJobModule>();
  }
  public override Task OnConfigureAsync(BonConfigurationContext context)
  {

    var preActions = context.Services.GetPreConfigureActions<IGlobalConfiguration>();
    
    context.Services.AddHangfire(config =>
    {
      config.UseInMemoryStorage();
      var x = context.Services.GetObjectOrNull<IContainer>();
      config.UseAutofacActivator(x);
      preActions.Configure(config);
    });
    
    context.Services.AddHangfireServer();

    context.Services.AddSingleton<IBonWorkerManager, HangfireWorkerManager>();
    return base.OnConfigureAsync(context);
  }

  public override Task OnPreApplicationAsync(BonContext context)
  {
    context.Application.UseHangfireDashboard();
    return base.OnPreApplicationAsync(context);
  }
}
