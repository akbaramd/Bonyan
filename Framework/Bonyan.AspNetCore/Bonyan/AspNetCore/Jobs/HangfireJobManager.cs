using Hangfire;

namespace Bonyan.AspNetCore.Jobs;

public class HangfireJobManager : IOptimumJobsManager
{
  private readonly IRecurringJobManager _recurringJobManager;
  private readonly IBackgroundJobClient _backgroundJobClient;

  public HangfireJobManager(IRecurringJobManager recurringJobManager, IBackgroundJobClient backgroundJobClient)
  {
    _recurringJobManager = recurringJobManager;
    _backgroundJobClient = backgroundJobClient;
  }

  // Add and register cron jobs
  public void AddCronJob<TJob>(string cronExpression, IServiceScope scope) where TJob : IJob
  {
    var jobInstance = scope.ServiceProvider.GetRequiredService<TJob>();
    _recurringJobManager.AddOrUpdate(
      typeof(TJob).Name,
      () => jobInstance.ExecuteAsync(),
      cronExpression
    );
    Console.WriteLine($"Registered cron job: {typeof(TJob).Name} with cron expression: {cronExpression}");
  }

  // Add and register background jobs
  public void AddBackgroundJob<TJob>(IServiceScope scope) where TJob : IJob
  {
    var jobInstance = scope.ServiceProvider.GetRequiredService<TJob>();
    _backgroundJobClient.Enqueue(() => jobInstance.ExecuteAsync());
    Console.WriteLine($"Registered background job: {typeof(TJob).Name}");
  }

  public void RegisterJobs(IEnumerable<Type> jobTypes, IServiceScope scope, BonyanServiceInfo serviceInfo)
  {
    // Use specific AddCronJob and AddBackgroundJob methods
    foreach (var jobType in jobTypes)
    {
      // You can handle this logic based on conditions or inputs from the builder
    }
  }
}