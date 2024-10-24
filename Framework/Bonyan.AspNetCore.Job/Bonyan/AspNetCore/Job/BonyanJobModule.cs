using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Modularity.Attributes;
using Microsoft.Extensions.Options;

namespace Bonyan.AspNetCore.Job;

[DependOn(typeof(BonyanAspNetCoreModule))]
public class BonyanJobModule : WebModule
{
  public override Task OnConfigureAsync(ModularityContext context)
  {
    context.Services.AddSingleton<IOptimumJobsManager, InMemoryJobManager>();
    return base.OnConfigureAsync(context);
  }

  public override Task OnPostConfigureAsync(ModularityContext context)
  {
    
    JobConfiguration jobConfiguration;

    var options = context.RequireService<IOptions<JobConfiguration>>();
    jobConfiguration = options.Value;

    if (jobConfiguration != null)
    {
      foreach (var backgroundJobType in jobConfiguration.BackgroundJobTypes)
      {
        context.Services.AddTransient(backgroundJobType);
      }
      
      foreach (var cronJobType in jobConfiguration.CronJobTypes)
      {
        context.Services.AddTransient(cronJobType.Item1);
      }
    }
    return base.OnPostConfigureAsync(context);
  }
  
  public override Task OnPostApplicationAsync(ModularityApplicationContext context)
  {
    var options = context.RequireService<IOptions<JobConfiguration>>();
    var jobOptions = options.Value;
    if (jobOptions != null)
    {
      var jobManager = context.GetService<IOptimumJobsManager>();
              
      if (jobManager == null)
      {
        throw new Exception(
          $"please add you current job Provider Module like: Hangfire or ...");
      }

    
      // Register cron jobs
      foreach (var (jobType, cronExpression) in jobOptions.CronJobTypes)
      {
        var addCronJobMethod = typeof(IOptimumJobsManager).GetMethod("AddCronJob")?.MakeGenericMethod(jobType);
        addCronJobMethod?.Invoke(jobManager, new object[] { cronExpression, context.BonyanApplication.Application.Services });
                    
      }

      // Register background jobs
      foreach (var jobType in jobOptions.BackgroundJobTypes)
      {
        var addBackgroundJobMethod = typeof(IOptimumJobsManager).GetMethod("AddBackgroundJob")?.MakeGenericMethod(jobType);
        addBackgroundJobMethod?.Invoke(jobManager, new object[] { context.BonyanApplication.Application.Services });
                    
      }
    }
    
    return base.OnPostApplicationAsync(context);
  }
}
