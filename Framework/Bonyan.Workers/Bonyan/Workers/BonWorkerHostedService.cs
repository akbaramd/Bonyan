using System.Reflection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bonyan.Workers
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

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting BonWorkerHostedService.");

            try
            {
                var workerTypes = _configuration.GetRegisteredWorkerTypes();

                if (workerTypes == null || !workerTypes.Any())
                {
                    _logger.LogWarning("No workers implementing IBonWorker were found in the configuration.");
                    return;
                }

                foreach (var workerType in workerTypes)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    // Check for CronJobAttribute
                    var cronAttribute = workerType.GetCustomAttribute<CronJobAttribute>();
                    var cronExpression = cronAttribute?.CronExpression;

                    if (!string.IsNullOrEmpty(cronExpression))
                    {
                        try
                        {
                            _workerManager.ScheduleRecurring(workerType, cronExpression);
                            _logger.LogInformation("Scheduled recurring job {JobType} with cron expression '{CronExpression}'", workerType.FullName, cronExpression);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error scheduling recurring job {JobType}", workerType.FullName);
                        }
                    }
                    else
                    {
                        try
                        {
                            _workerManager.Enqueue(workerType);
                            _logger.LogInformation("Enqueued background job {JobType}", workerType.FullName);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error enqueuing background job {JobType}", workerType.FullName);
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

            await Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping BonWorkerHostedService.");
            // No additional cleanup required for this implementation
            return Task.CompletedTask;
        }
    }
}
