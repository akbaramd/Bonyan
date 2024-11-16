using Bonyan.Modularity;
using Bonyan.Workers.Tests.Mock;
using Microsoft;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Bonyan.Workers.Tests
{
    public class BonWorkersModuleTests
    {
        private readonly IServiceCollection _services;
        private readonly BonConfigurationContext _configurationContext;
        private readonly Mock<BonWorkersModule> _moduleMock;

        public BonWorkersModuleTests()
        {
            _services = new ServiceCollection();
            _configurationContext = new BonConfigurationContext(_services);
            _moduleMock = new Mock<BonWorkersModule> { CallBase = true };
        }

        [Fact]
        public async Task OnPostConfigureAsync_Should_Add_Workers()
        {
            // Arrange
            var preConfigureActionCalled = false;
            _services.PreConfigure<BonWorkerConfiguration>(config =>
            {
                preConfigureActionCalled = true;
                config.RegisterWorker<TestWorkerA>();
            });

            // Act
            await _moduleMock.Object.OnPostConfigureAsync(_configurationContext);

            // Assert
            Assert.True(preConfigureActionCalled);

            var workerConfig = _services.BuildServiceProvider().GetService<BonWorkerConfiguration>();
            Assert.NotNull(workerConfig);

            var registeredWorkers = workerConfig.GetRegisteredWorkerTypes().ToList();
            Assert.Contains(typeof(TestWorkerA), registeredWorkers);
        }
    }
}