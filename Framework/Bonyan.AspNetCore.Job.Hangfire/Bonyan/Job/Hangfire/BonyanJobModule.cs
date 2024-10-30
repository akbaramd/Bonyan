using Bonyan.AspNetCore.Job;
using Bonyan.Modularity;
using Hangfire;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bonyan.Job.Hangfire;

[DependOn(typeof(BonyanJobModule))]
public class BonyanJobHangfireModule : WebModule
{
  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {

    context.Services.AddHangfire(config => config.UseInMemoryStorage());
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
