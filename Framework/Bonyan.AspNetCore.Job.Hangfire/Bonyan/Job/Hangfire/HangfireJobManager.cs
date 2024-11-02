using Bonyan.AspNetCore.Job;
using Bonyan.UnitOfWork;
using Hangfire;

namespace Bonyan.Job.Hangfire;

public class HangfireJobManager : IBonyanJobsManager
{

 

  // Add and register cron jobs
  public void AddCronJob<TJob>(TJob job,string cronExpression) where TJob : IJob
  {
    RecurringJob.AddOrUpdate(typeof(TJob).Name, () => job.ExecuteAsync(CancellationToken.None),
      cronExpression
    );
    Console.WriteLine($"Registered cron job: {typeof(TJob).Name} with cron expression: {cronExpression}");
  }

  // Add and register background jobs
  public void AddBackgroundJob<TJob>(TJob job) where TJob : IJob
  {
    BackgroundJob.Enqueue(() => job.ExecuteAsync(CancellationToken.None));
    Console.WriteLine($"Registered background job: {typeof(TJob).Name}");
  }

  
}