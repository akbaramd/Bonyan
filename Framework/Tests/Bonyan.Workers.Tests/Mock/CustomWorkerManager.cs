using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Bonyan.Workers;
using Microsoft.Extensions.DependencyInjection;

public class CustomWorkerManager : IBonWorkerManager, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentQueue<Type> _jobQueue = new ConcurrentQueue<Type>();
    private readonly ConcurrentDictionary<string, ScheduledJob> _scheduledJobs = new ConcurrentDictionary<string, ScheduledJob>();
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private readonly Task _processingTask;
    private readonly Task _schedulingTask;

    public CustomWorkerManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        // Start processing tasks
        _processingTask = Task.Run(ProcessJobsAsync);
        _schedulingTask = Task.Run(ProcessScheduledJobsAsync);
    }

    public void Enqueue(Type jobType, string jobId = null)
    {
        ValidateJobType(jobType);
        _jobQueue.Enqueue(jobType);
    }

    public void Enqueue<TWorker>(string jobId = null) where TWorker : IBonWorker
    {
        Enqueue(typeof(TWorker), jobId);
    }

    public void ScheduleRecurring(Type jobType, string scheduleExpression, string jobId = null)
    {
        ValidateJobType(jobType);

        if (string.IsNullOrWhiteSpace(scheduleExpression))
            throw new ArgumentException("Schedule expression cannot be null or whitespace.", nameof(scheduleExpression));

        var id = jobId ?? jobType.FullName;

        var scheduledJob = new ScheduledJob
        {
            JobType = jobType,
            CronExpression = scheduleExpression,
            NextRunTime = GetNextRunTime(scheduleExpression)
        };

        _scheduledJobs[id] = scheduledJob;
    }

    public void ScheduleRecurring<TWorker>(string scheduleExpression, string jobId = null) where TWorker : IBonWorker
    {
        ScheduleRecurring(typeof(TWorker), scheduleExpression, jobId);
    }

    public void CancelRecurring(string jobId)
    {
        if (string.IsNullOrWhiteSpace(jobId))
            throw new ArgumentException("Job ID cannot be null or whitespace.", nameof(jobId));

        _scheduledJobs.TryRemove(jobId, out _);
    }

    private void ValidateJobType(Type jobType)
    {
        if (jobType == null)
            throw new ArgumentNullException(nameof(jobType));

        if (!typeof(IBonWorker).IsAssignableFrom(jobType))
            throw new ArgumentException($"Job type {jobType.FullName} does not implement IBonWorker.", nameof(jobType));

        // Ensure the worker is registered in the service provider
        if (_serviceProvider.GetService(jobType) == null)
            throw new InvalidOperationException($"Worker type {jobType.FullName} is not registered in the service provider.");
    }

    private async Task ProcessJobsAsync()
    {
        while (!_cts.Token.IsCancellationRequested)
        {
            if (_jobQueue.TryDequeue(out var jobType))
            {
                await ExecuteJobAsync(jobType);
            }
            else
            {
                await Task.Delay(500, _cts.Token);
            }
        }
    }

    private async Task ProcessScheduledJobsAsync()
    {
        while (!_cts.Token.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;

            foreach (var kvp in _scheduledJobs)
            {
                var scheduledJob = kvp.Value;

                if (scheduledJob.NextRunTime <= now)
                {
                    await ExecuteJobAsync(scheduledJob.JobType);

                    // Calculate next run time
                    scheduledJob.NextRunTime = GetNextRunTime(scheduledJob.CronExpression);
                }
            }

            await Task.Delay(500, _cts.Token);
        }
    }

    private DateTime GetNextRunTime(string cronExpression)
    {
        // Simple implementation: Run every minute
        // You can use a library like NCrontab for accurate cron parsing
        return DateTime.UtcNow.AddMinutes(1);
    }

    private async Task ExecuteJobAsync(Type jobType)
    {
        using var scope = _serviceProvider.CreateScope();
        var worker = (IBonWorker)scope.ServiceProvider.GetRequiredService(jobType);

        try
        {
            await worker.ExecuteAsync(_cts.Token);
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., logging)
            Console.WriteLine($"Error executing job {jobType.FullName}: {ex.Message}");
        }
    }

    public void Dispose()
    {
        _cts.Cancel();

        try
        {
            Task.WaitAll(new[] { _processingTask, _schedulingTask }, TimeSpan.FromSeconds(5));
        }
        catch (AggregateException)
        {
            // Ignore exceptions during disposal
        }
        _cts.Dispose();
    }

    private class ScheduledJob
    {
        public Type JobType { get; set; }
        public string CronExpression { get; set; }
        public DateTime NextRunTime { get; set; }
    }
}
