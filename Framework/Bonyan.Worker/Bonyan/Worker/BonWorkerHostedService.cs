using System;
using System.Threading;
using System.Threading.Tasks;
using Bonyan.AspNetCore.Job;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bonyan.Worker
{
    public class BonWorkerHostedService : IHostedService
    {
        private readonly IBonWorkerManager _workerManager;
        private readonly BonWorkerConfiguration _configuration;
        private readonly ILogger<BonWorkerHostedService> _logger;

        public BonWorkerHostedService(
            IBonWorkerManager workerManager,
            BonWorkerConfiguration configuration,
            ILogger<BonWorkerHostedService> logger)
        {
            _workerManager = workerManager ?? throw new ArgumentNullException(nameof(workerManager));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting BonWorkerHostedService.");

            try
            {
                // Schedule cron jobs and enqueue background jobs
                foreach (var registration in _configuration.GetWorkerRegistrations())
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var jobType = registration.ImplementationType;

                    if (!string.IsNullOrEmpty(registration.CronExpression))
                    {
                        var cronExpression = registration.CronExpression;

                        try
                        {
                            _workerManager.ScheduleRecurring(jobType, cronExpression);
                            _logger.LogInformation("Scheduled recurring job {JobType} with cron expression '{CronExpression}'", jobType.FullName, cronExpression);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error scheduling recurring job {JobType}", jobType.FullName);
                        }
                    }
                    else
                    {
                        try
                        {
                            _workerManager.Enqueue(jobType);
                            _logger.LogInformation("Enqueued background job {JobType}", jobType.FullName);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error enqueuing background job {JobType}", jobType.FullName);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("StartAsync operation was canceled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while starting BonWorkerHostedService.");
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping BonWorkerHostedService.");
            // No additional cleanup required for Hangfire
            return Task.CompletedTask;
        }
    }
}
