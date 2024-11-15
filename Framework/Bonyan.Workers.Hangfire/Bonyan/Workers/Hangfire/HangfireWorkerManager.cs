using System.Reflection;
using Hangfire;
using Hangfire.States;
using Microsoft.Extensions.Logging;

namespace Bonyan.Workers.Hangfire
{
    /// <summary>
    /// Manages background and recurring jobs using Hangfire.
    /// Ensures that jobs are registered with the service provider before being scheduled or enqueued.
    /// </summary>
    internal class HangfireWorkerManager : IBonWorkerManager
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<HangfireWorkerManager> _logger;

        public HangfireWorkerManager(
            IBackgroundJobClient backgroundJobClient,
            IRecurringJobManager recurringJobManager,
            IServiceProvider serviceProvider,
            ILogger<HangfireWorkerManager> logger)
        {
            _backgroundJobClient = backgroundJobClient ?? throw new ArgumentNullException(nameof(backgroundJobClient));
            _recurringJobManager = recurringJobManager ?? throw new ArgumentNullException(nameof(recurringJobManager));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Enqueue<TWorker>(string? jobId = null) where TWorker : IBonWorker
        {
            Enqueue(typeof(TWorker), jobId);
        }

        public void Enqueue(Type jobType, string? jobId = null)
        {
            EnsureWorkerIsValid(jobType);

            try
            {
                var method = GetExecuteAsyncMethod(jobType);

                var job = new global::Hangfire.Common.Job(jobType, method, CancellationToken.None);

                var backgroundJobId = _backgroundJobClient.Create(job, new EnqueuedState());

                _logger.LogInformation("Enqueued background job of type {JobType} with Job ID {JobId}.", jobType.FullName, backgroundJobId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to enqueue background job of type {JobType}.", jobType.FullName);
                throw;
            }
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

            var recurringJobId = jobId ?? jobType.FullName;

            try
            {
                var method = GetExecuteAsyncMethod(jobType);

                var job = new global::Hangfire.Common.Job(jobType, method, CancellationToken.None);

                _recurringJobManager.AddOrUpdate(recurringJobId, job, scheduleExpression);

                _logger.LogInformation("Scheduled recurring job {JobId} of type {JobType} with schedule '{ScheduleExpression}'.", recurringJobId, jobType.FullName, scheduleExpression);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to schedule recurring job {JobId} of type {JobType}.", recurringJobId, jobType.FullName);
                throw;
            }
        }

        public void CancelRecurring(string jobId)
        {
            if (string.IsNullOrWhiteSpace(jobId))
            {
                throw new ArgumentException("Job ID cannot be null or whitespace.", nameof(jobId));
            }

            try
            {
                _recurringJobManager.RemoveIfExists(jobId);
                _logger.LogInformation("Cancelled recurring job with ID {JobId}.", jobId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cancel recurring job with ID {JobId}.", jobId);
                throw;
            }
        }

        private void EnsureWorkerIsValid(Type workerType)
        {
            if (workerType == null)
            {
                throw new ArgumentNullException(nameof(workerType));
            }

            if (!typeof(IBonWorker).IsAssignableFrom(workerType))
            {
                throw new ArgumentException($"Type {workerType.FullName} does not implement IBonWorker.", nameof(workerType));
            }

            if (_serviceProvider.GetService(workerType) == null)
            {
                throw new InvalidOperationException($"Worker of type {workerType.FullName} is not registered with the service provider.");
            }
        }

        private static MethodInfo GetExecuteAsyncMethod(Type jobType)
        {
            var method = jobType.GetMethod(nameof(IBonWorker.ExecuteAsync), new[] { typeof(CancellationToken) });
            if (method == null)
            {
                throw new InvalidOperationException($"Method {nameof(IBonWorker.ExecuteAsync)} not found on type {jobType.FullName}.");
            }
            return method;
        }
    }
}
