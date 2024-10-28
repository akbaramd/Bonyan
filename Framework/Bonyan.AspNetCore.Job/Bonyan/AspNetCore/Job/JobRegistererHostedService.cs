using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Bonyan.AspNetCore.Job;

public class JobRegistererHostedService(IServiceProvider serviceProvider) : IHostedService
{
  public Task StartAsync(CancellationToken cancellationToken)
  {
    var jobManager = serviceProvider.GetService<IBonyanJobsManager>();

    if (jobManager == null)
    {
      throw new Exception(
        $"Please add your current job provider module, like: Hangfire or others.");
    }

    // Retrieve all services implementing IJob
    var jobServices = serviceProvider.GetServices<IJob>();

    foreach (var jobService in jobServices)
    {
      var jobType = jobService.GetType();
      var cronAttribute = jobType.GetCustomAttribute<CronJobAttribute>();

      if (cronAttribute != null)
      {
        // Register as a cron job using the cron expression from the attribute
        var cronExpression = cronAttribute.CronExpression;
        var addCronJobMethod = typeof(IBonyanJobsManager)
          .GetMethod("AddCronJob")?.MakeGenericMethod(jobType);
        addCronJobMethod?.Invoke(jobManager, new object[] { cronExpression });
      }
      else
      {
        // Register as a background job
        var addBackgroundJobMethod = typeof(IBonyanJobsManager)
          .GetMethod("AddBackgroundJob")?.MakeGenericMethod(jobType);
        addBackgroundJobMethod?.Invoke(jobManager, new object[] { });
      }
    }

    return Task.CompletedTask;
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    // If you have any cleanup logic or if you need to stop any jobs, you can do it here.
    // For now, we just return a completed task since there's nothing to clean up.
    return Task.CompletedTask;
  }
}
