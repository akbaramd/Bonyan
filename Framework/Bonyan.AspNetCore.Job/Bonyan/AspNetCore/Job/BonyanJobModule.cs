using Bonyan.Modularity;
using Module = Bonyan.Modularity.Abstractions.Module;

namespace Bonyan.AspNetCore.Job;

public class BonyanJobModule : Module
{
  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {
    context.Services.AddSingleton<IBonyanJobsManager, InMemoryJobManager>();
    
    context.Services.AddHostedService<JobRegistererHostedService>();
    return base.OnConfigureAsync(context);
  }

  public override Task OnPostConfigureAsync(ServiceConfigurationContext context)
  {
    return base.OnPostConfigureAsync(context);
  }
  


}