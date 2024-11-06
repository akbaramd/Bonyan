using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Quartz;
using Quartz.AspNetCore;

namespace Bonyan.AspNetCore.Job;

public class BonyanJobModule : Module
{
  public override Task OnConfigureAsync(ServiceConfigurationContext context)
  {
    
    context.Services.AddQuartz(q =>
    {
      // base Quartz scheduler, job and trigger configuration
    });
    
    context.Services.AddQuartzServer(options =>
    {
      // when shutting down we want jobs to complete gracefully
      options.WaitForJobsToComplete = true;
    });
    
    context.Services.AddSingleton<IBonyanJobsManager, InMemoryJobManager>();
    
    context.Services.AddHostedService<JobRegistererHostedService>();
    return base.OnConfigureAsync(context);
  }

  public override Task OnPostConfigureAsync(ServiceConfigurationContext context)
  {
    return base.OnPostConfigureAsync(context);
  }
  


}