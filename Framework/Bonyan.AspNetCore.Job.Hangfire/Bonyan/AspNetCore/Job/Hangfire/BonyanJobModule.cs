using Bonyan.Modularity;
using Bonyan.Modularity.Attributes;
using Hangfire;
using Module = Bonyan.Modularity.Abstractions.Module;

namespace Bonyan.AspNetCore.Job.Hangfire;

[DependOn(typeof(BonyanJobModule))]
public class BonyanJobHangfireModule : WebModule
{
  public override Task OnConfigureAsync(ModularityContext context)
  {

    context.Services.AddHangfire(config => config.UseInMemoryStorage());
    context.Services.AddHangfireServer();
    
    context.Services.AddSingleton<IOptimumJobsManager, HangfireJobManager>();
    return base.OnConfigureAsync(context);
  }

  public override Task OnPostInitializeAsync(ModularityInitializedContext context)
  {
   
    return base.OnPostInitializeAsync(context);
  }


  public override Task OnPreApplicationAsync(ModularityApplicationContext app)
  {
    app.BonyanApplication.Application.UseHangfireDashboard();
    return base.OnPreApplicationAsync(app);
  }
}
