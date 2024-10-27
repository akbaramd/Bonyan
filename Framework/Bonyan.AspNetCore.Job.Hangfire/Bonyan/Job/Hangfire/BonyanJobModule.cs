using Bonyan.AspNetCore.Job;
using Bonyan.Modularity;
using Bonyan.Modularity.Attributes;
using Hangfire;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bonyan.Job.Hangfire;

[DependOn(typeof(BonyanJobModule))]
public class BonyanJobHangfireModule : WebModule
{
  public override Task OnConfigureAsync(ModularityContext context)
  {

    context.Services.AddHangfire(config => config.UseInMemoryStorage());
    context.Services.AddHangfireServer();
    
    context.Services.Replace(ServiceDescriptor.Singleton<IBonyanJobsManager, HangfireJobManager>());
    return base.OnConfigureAsync(context);
  }

  public override Task OnPreApplicationAsync(ModularityApplicationContext app)
  {
    app.WebApplication.UseHangfireDashboard();
    return base.OnPreApplicationAsync(app);
  }
}
