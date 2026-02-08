using Bonyan.Modularity;
using Bonyan.Workers.Tests.Mock;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Bonyan.Workers.Tests
{
    public class BonWorkerHostedServiceTests
    {
        private readonly IServiceCollection _services;
        private readonly IServiceProvider _serviceProvider;
        private readonly Mock<IBonWorkerManager> _workerManagerMock;
        private readonly Mock<ILogger<BonWorkerHostedService>> _loggerMock;
        private readonly BonWorkerConfiguration _configuration;

        public BonWorkerHostedServiceTests()
        {
            _services = new ServiceCollection();
            _workerManagerMock = new Mock<IBonWorkerManager>();
            _loggerMock = new Mock<ILogger<BonWorkerHostedService>>();
            _configuration = new BonWorkerConfiguration(new BonPostConfigurationContext(_services));
            _serviceProvider = _services.BuildServiceProvider();
        }

        [Fact]
        public async Task StartAsync_Should_Enqueue_Workers()
        {
            // Arrange
            _configuration.RegisterWorker<TestWorkerA>();
            var hostedService = new BonWorkerHostedService(_workerManagerMock.Object, _configuration, _loggerMock.Object);

            // Act
            await hostedService.StartAsync(CancellationToken.None);

            // Assert
            _workerManagerMock.Verify(wm => wm.Enqueue(typeof(TestWorkerA), null), Times.Once);
        }

        [Fact]
        public async Task StartAsync_Should_Schedule_Recurring_Workers()
        {
            // Arrange
            _configuration.RegisterWorker<CronWorker>();
            var hostedService = new BonWorkerHostedService(_workerManagerMock.Object, _configuration, _loggerMock.Object);

            // Act
            await hostedService.StartAsync(CancellationToken.None);

            // Assert
            _workerManagerMock.Verify(wm => wm.ScheduleRecurring(typeof(CronWorker), "* * * * *", null), Times.Once);
        }

        [Fact]
        public async Task StartAsync_Should_Log_Warning_When_No_Workers()
        {
            // Arrange
            var hostedService = new BonWorkerHostedService(_workerManagerMock.Object, _configuration, _loggerMock.Object);

            // Act
            await hostedService.StartAsync(CancellationToken.None);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No workers implementing IBonWorker were found in the configuration.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task StopAsync_Should_Complete_Without_Errors()
        {
            // Arrange
            var hostedService = new BonWorkerHostedService(_workerManagerMock.Object, _configuration, _loggerMock.Object);

            // Act
            await hostedService.StopAsync(CancellationToken.None);

            // Assert
            // No exceptions should be thrown
        }

        [Fact]
        public async Task StartAsync_Should_Handle_Exceptions()
        {
            // Arrange
            _configuration.RegisterWorker<TestWorkerA>();

            _workerManagerMock
                .Setup(wm => wm.Enqueue(It.IsAny<Type>(), null))
                .Throws(new Exception("Test exception"));

            var hostedService = new BonWorkerHostedService(_workerManagerMock.Object, _configuration, _loggerMock.Object);

            // Act
            await hostedService.StartAsync(CancellationToken.None);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error enqueuing background job")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
