using System.Collections.Concurrent;
using NCrontab;

namespace Bonyan.AspNetCore.Job
{
    public class InMemoryJobManager : IBonyanJobsManager, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InMemoryJobManager> _logger;
        private readonly ConcurrentDictionary<string, Timer?> _timers = new ConcurrentDictionary<string, Timer?>();

        public InMemoryJobManager(IServiceProvider serviceProvider, ILogger<InMemoryJobManager> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        // Add and register cron jobs
        public void AddCronJob<TJob>(string cronExpression) where TJob : IJob
        {
            try
            {
                var schedule = CrontabSchedule.Parse(cronExpression);
                var nextOccurrence = schedule.GetNextOccurrence(DateTime.Now);
                var jobId = typeof(TJob).FullName ?? Guid.NewGuid().ToString();

                Timer? timer = null;
                timer = new Timer(async _ =>
                {
                    try
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var job = scope.ServiceProvider.GetRequiredService<TJob>();
                        await job.ExecuteAsync();
                        _logger.LogInformation("Executed cron job: {JobName} at {Time}", typeof(TJob).Name, DateTime.Now);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error executing cron job: {JobName}", typeof(TJob).Name);
                    }
                    finally
                    {
                        // Reschedule the next occurrence
                        var next = schedule.GetNextOccurrence(DateTime.Now);
                        var delay = next - DateTime.Now;
                        if (delay.TotalMilliseconds < 0)
                            delay = TimeSpan.Zero;

                        timer?.Change(delay, Timeout.InfiniteTimeSpan);
                    }
                }, null, nextOccurrence - DateTime.Now, Timeout.InfiniteTimeSpan);

                if (!_timers.TryAdd(jobId, timer))
                {
                    _logger.LogWarning("Cron job already exists: {JobName}", typeof(TJob).Name);
                    timer.Dispose();
                }
                else
                {
                    _logger.LogInformation("Registered cron job: {JobName} with cron expression: {CronExpression}", typeof(TJob).Name, cronExpression);
                }
            }
          
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering cron job: {JobName}", typeof(TJob).Name);
                throw;
            }
        }

        // Add and register background jobs
        public void AddBackgroundJob<TJob>() where TJob : IJob
        {
            try
            {
                Task.Run(async () =>
                {
                    try
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var job = scope.ServiceProvider.GetRequiredService<TJob>();
                        await job.ExecuteAsync();
                        _logger.LogInformation("Executed background job: {JobName} at {Time}", typeof(TJob).Name, DateTime.Now);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error executing background job: {JobName}", typeof(TJob).Name);
                    }
                });

                _logger.LogInformation("Registered background job: {JobName}", typeof(TJob).Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering background job: {JobName}", typeof(TJob).Name);
                throw;
            }
        }

        public void Dispose()
        {
            foreach (var timer in _timers.Values)
            {
                timer?.Dispose();
            }
        }
    }
}
