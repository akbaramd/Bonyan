using Bonyan.AspNetCore.Job;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using Bonyan.Exceptions;

namespace Bonyan.Job.Hangfire
{
    /// <summary>
    /// Manages background and recurring jobs using Hangfire.
    /// Ensures that jobs are registered with the service provider before being scheduled or enqueued.
    /// </summary>
    public class HangfireWorkerManager : IBonWorkerManager
    {
        private readonly IBackgroundJobClientV2 _clientV2;
        private readonly IRecurringJobManagerV2 _recurringJobManager;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="HangfireWorkerManager"/> class.
        /// </summary>
        /// <param name="clientV2">Client for enqueuing background jobs.</param>
        /// <param name="serviceProvider">Service provider to verify job registration.</param>
        /// <param name="recurringJobManager">Manager for scheduling recurring jobs.</param>
        /// <exception cref="ArgumentNullException">Thrown if any dependency is null.</exception>
        public HangfireWorkerManager(IBackgroundJobClientV2 clientV2, IServiceProvider serviceProvider, IRecurringJobManagerV2 recurringJobManager)
        {
            _clientV2 = clientV2 ?? throw new ArgumentNullException(nameof(clientV2), "Background job client cannot be null.");
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider), "Service provider cannot be null.");
            _recurringJobManager = recurringJobManager ?? throw new ArgumentNullException(nameof(recurringJobManager), "Recurring job manager cannot be null.");
        }

        /// <summary>
        /// Enqueues a background job for the specified worker type.
        /// </summary>
        /// <typeparam name="TWorker">The worker type that implements IBonWorker.</typeparam>
        /// <param name="jobId">Optional job identifier. If null, the worker type name is used.</param>
        /// <exception cref="BonHangfireWorkerCreationException">Thrown if the job type is not registered or if enqueuing fails.</exception>
        public void Enqueue<TWorker>(string? jobId = null) where TWorker : IBonWorker
        {
            // Verify that the job type is registered with the service provider
            if (_serviceProvider.GetService<TWorker>() == null)
            {
                throw new HangfireJobException(
                    $"Job of type {typeof(TWorker).Name} is not registered in the service provider.");
            }

            // Attempt to enqueue the job
            var jobCreationId = _clientV2.Enqueue<TWorker>(
                j => j.ExecuteAsync(CancellationToken.None)
            );

            if (string.IsNullOrEmpty(jobCreationId))
            {
                throw new HangfireJobException(
                    $"Failed to enqueue job of type {typeof(TWorker).Name}. Please ensure the job type is correctly registered.");
            }
        }

        /// <summary>
        /// Schedules a recurring job for the specified worker type.
        /// </summary>
        /// <typeparam name="TWorker">The worker type that implements IBonWorker.</typeparam>
        /// <param name="cronExpression">A cron expression that defines the schedule.</param>
        /// <param name="jobId">Optional job identifier. If null, the worker type name is used.</param>
        /// <exception cref="ArgumentException">Thrown if the cron expression is null or whitespace.</exception>
        /// <exception cref="BonHangfireWorkerCreationException">Thrown if the job type is not registered.</exception>
        public void ScheduleRecurring<TWorker>(string cronExpression, string? jobId = null) where TWorker : IBonWorker
        {
            var jobIdentifier = jobId ?? typeof(TWorker).FullName;

            if (string.IsNullOrWhiteSpace(cronExpression))
            {
                throw new ArgumentException("Cron expression cannot be null or whitespace.", nameof(cronExpression));
            }

            // Verify that the job type is registered with the service provider
            if (_serviceProvider.GetService<TWorker>() == null)
            {
                throw new HangfireJobException(
                    $"Recurring job of type {typeof(TWorker).Name} is not registered in the service provider.");
            }

            // Attempt to schedule the recurring job
            _recurringJobManager.AddOrUpdate<TWorker>(
                jobIdentifier,
                j => j.ExecuteAsync(CancellationToken.None),
                cronExpression
            );
        }

        /// <summary>
        /// Cancels a recurring job by its job identifier.
        /// </summary>
        /// <param name="jobId">The identifier of the recurring job to cancel.</param>
        /// <exception cref="ArgumentException">Thrown if jobId is null or whitespace.</exception>
        /// <exception cref="BonHangfireWorkerCreationException">Thrown if the job does not exist.</exception>
        public void CancelRecurring(string jobId)
        {
            if (string.IsNullOrWhiteSpace(jobId))
            {
                throw new ArgumentException("Job ID cannot be null or whitespace.", nameof(jobId));
            }

            RecurringJob.RemoveIfExists(jobId);
        }
    }


}
