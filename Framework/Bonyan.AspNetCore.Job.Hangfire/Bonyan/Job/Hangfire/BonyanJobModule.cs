using Autofac;
using Autofac.Core.Lifetime;
using Bonyan.AspNetCore.Job;
using Bonyan.Modularity;
using Hangfire;
using Microsoft;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bonyan.Job.Hangfire;


public class BonyanJobHangfireModule : WebModule
{
  public BonyanJobHangfireModule()
  {
    DependOn<BonyanJobModule>();
  }
  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {

    context.Services.AddHangfire(config =>
    {
      config.UseInMemoryStorage();
      var x = context.Services.GetObjectOrNull<IContainer>();
      config.UseAutofacActivator(x);
    });
    context.Services.AddHangfireServer();
    
    context.Services.Replace(ServiceDescriptor.Singleton<IBonyanJobsManager, HangfireJobManager>());
    return base.OnConfigureAsync(context);
  }

  public override Task OnPreApplicationAsync(ApplicationContext context)
  {
    context.Application.UseHangfireDashboard();
    return base.OnPreApplicationAsync(context);
  }
}
