using System.Reflection;
using Bonyan.Exceptions;
using Bonyan.UnitOfWork;

namespace Bonyan.AspNetCore.Job;

public class JobRegistererHostedService(IServiceProvider serviceProvider) : IHostedService
{
  public Task StartAsync(CancellationToken cancellationToken)
  {
    var jobManager = serviceProvider.GetService<IBonyanJobsManager>();

    if (jobManager == null)
    {
      throw new BonyanException(
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
        jobManager.AddCronJob(jobService,cronExpression);
      }
      else
      {
        // Register as a background job
        jobManager.AddBackgroundJob(jobService);
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
