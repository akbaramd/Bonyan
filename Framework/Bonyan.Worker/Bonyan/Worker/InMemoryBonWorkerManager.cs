using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Bonyan.AspNetCore.Job;
using NCrontab;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Worker
{
    public class InMemoryBonWorkerManager : IBonWorkerManager, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentQueue<Type> _backgroundJobs = new ConcurrentQueue<Type>();
        private readonly ConcurrentDictionary<string, RecurringJob> _recurringJobs = new ConcurrentDictionary<string, RecurringJob>();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly Task _backgroundTask;
        private readonly Task _recurringTask;

        public InMemoryBonWorkerManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            _backgroundTask = StartBackgroundProcessing();
            _recurringTask = StartRecurringJobs();
        }

        public void Enqueue<TWorker>(string? jobId = null) where TWorker : IBonWorker
        {
            Enqueue(typeof(TWorker), jobId);
        }

        public void Enqueue(Type jobType, string? jobId = null)
        {
            EnsureWorkerIsValid(jobType);
            _backgroundJobs.Enqueue(jobType);
        }

        public void ScheduleRecurring<TWorker>(string scheduleExpression, string? jobId = null) where TWorker : IBonWorker
        {
            ScheduleRecurring(typeof(TWorker), scheduleExpression, jobId);
        }

        public void ScheduleRecurring(Type jobType, string scheduleExpression, string? jobId = null)
        {
            if (string.IsNullOrWhiteSpace(scheduleExpression))
            {
                throw new ArgumentException("Schedule expression cannot be null or whitespace.", nameof(scheduleExpression));
            }

            EnsureWorkerIsValid(jobType);

            var id = jobId ?? jobType.FullName!;
            var schedule = CrontabSchedule.Parse(scheduleExpression);

            var recurringJob = new RecurringJob
            {
                JobType = jobType,
                Schedule = schedule,
                NextRun = schedule.GetNextOccurrence(DateTime.UtcNow)
            };

            _recurringJobs[id] = recurringJob;
        }

        public void CancelRecurring(string jobId)
        {
            if (string.IsNullOrWhiteSpace(jobId))
            {
                throw new ArgumentException("Job ID cannot be null or whitespace.", nameof(jobId));
            }

            _recurringJobs.TryRemove(jobId, out _);
        }

        private Task StartBackgroundProcessing()
        {
            return Task.Run(async () =>
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    if (_backgroundJobs.TryDequeue(out var jobType))
                    {
                        await ExecuteJobAsync(jobType, _cts.Token);
                    }
                    else
                    {
                        await Task.Delay(1000, _cts.Token);
                    }
                }
            }, _cts.Token);
        }

        private Task StartRecurringJobs()
        {
            return Task.Run(async () =>
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    var now = DateTime.UtcNow;
                    foreach (var kvp in _recurringJobs)
                    {
                        var recurringJob = kvp.Value;
                        if (recurringJob.NextRun <= now)
                        {
                            await ExecuteJobAsync(recurringJob.JobType, _cts.Token);
                            recurringJob.NextRun = recurringJob.Schedule.GetNextOccurrence(now);
                        }
                    }
                    await Task.Delay(1000, _cts.Token);
                }
            }, _cts.Token);
        }

        private async Task ExecuteJobAsync(Type jobType, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var job = (IBonWorker)scope.ServiceProvider.GetRequiredService(jobType);

            try
            {
                await job.ExecuteAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                // Handle exceptions (log them, etc.)
                // For production code, consider logging the exception
                Console.WriteLine($"Error executing job {jobType.FullName}: {ex}");
            }
        }

        private void EnsureWorkerIsValid(Type jobType)
        {
            if (jobType == null)
            {
                throw new ArgumentNullException(nameof(jobType));
            }

            if (!typeof(IBonWorker).IsAssignableFrom(jobType))
            {
                throw new ArgumentException($"Job type {jobType.FullName} does not implement IBonWorker.", nameof(jobType));
            }

            // Verify that the job type is registered with the service provider
            if (_serviceProvider.GetService(jobType) == null)
            {
                throw new InvalidOperationException($"Worker of type {jobType.FullName} is not registered with the service provider.");
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            try
            {
                Task.WaitAll(new[] { _backgroundTask, _recurringTask }, TimeSpan.FromSeconds(5));
            }
            catch (AggregateException)
            {
                // Ignore exceptions during disposal
            }
            _cts.Dispose();
        }

        private class RecurringJob
        {
            public Type JobType { get; set; }
            public CrontabSchedule Schedule { get; set; }
            public DateTime NextRun { get; set; }
        }
    }
}
