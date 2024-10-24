using Hangfire;

namespace Bonyan.AspNetCore.Job.Hangfire;

public class HangfireJobManager : IOptimumJobsManager
{
  private readonly IRecurringJobManager _recurringJobManager;
  private readonly IBackgroundJobClient _backgroundJobClient;
  private readonly IServiceProvider _serviceProvider;
  public HangfireJobManager(IRecurringJobManager recurringJobManager, IBackgroundJobClient backgroundJobClient, IServiceProvider serviceProvider)
  {
    _recurringJobManager = recurringJobManager;
    _backgroundJobClient = backgroundJobClient;
    _serviceProvider = serviceProvider;
  }

  // Add and register cron jobs
  public void AddCronJob<TJob>(string cronExpression) where TJob : IJob
  {
    var scope = _serviceProvider.CreateScope().ServiceProvider;
    var jobInstance = scope.GetRequiredService<TJob>();
    _recurringJobManager.AddOrUpdate(typeof(TJob).Name, () => jobInstance.ExecuteAsync(CancellationToken.None),
      cronExpression
    );
    Console.WriteLine($"Registered cron job: {typeof(TJob).Name} with cron expression: {cronExpression}");
  }

  // Add and register background jobs
  public void AddBackgroundJob<TJob>() where TJob : IJob
  {
    var scope = _serviceProvider.CreateScope().ServiceProvider;
    var jobInstance = scope.GetRequiredService<TJob>();
    _backgroundJobClient.Enqueue(() => jobInstance.ExecuteAsync(CancellationToken.None));
    Console.WriteLine($"Registered background job: {typeof(TJob).Name}");
  }

  
}