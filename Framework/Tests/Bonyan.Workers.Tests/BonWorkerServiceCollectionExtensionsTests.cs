using System;
using System.Linq;
using Bonyan.Modularity;
using Bonyan.Tests.Workers.Mock;
using Bonyan.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Bonyan.Tests.Workers
{
    public class BonWorkerServiceCollectionExtensionsTests
    {
        private readonly IServiceCollection _services;
        private readonly BonConfigurationContext _configurationContext;

        public BonWorkerServiceCollectionExtensionsTests()
        {
            _services = new ServiceCollection();
            _configurationContext = new BonConfigurationContext(_services);
        }

        [Fact]
        public void AddWorkers_Should_Register_Services()
        {
            // Act
            _configurationContext.AddWorkers(config =>
            {
                config.RegisterWorker<TestWorkerA>();
            });

            // Build the service provider
            var serviceProvider = _services.BuildServiceProvider();

            // Assert
            // Verify that IBonWorkerManager is registered
            var workerManager = serviceProvider.GetService<IBonWorkerManager>();
            Assert.NotNull(workerManager);
            Assert.IsType<InMemoryBonWorkerManager>(workerManager);

            // Verify that BonWorkerHostedService is registered
            var hostedServiceDescriptor = _services.FirstOrDefault(sd => sd.ServiceType == typeof(IHostedService) && sd.ImplementationType == typeof(BonWorkerHostedService));
            Assert.NotNull(hostedServiceDescriptor);
        }

        [Fact]
        public void AddWorkers_Should_Not_Override_Existing_IBonWorkerManager()
        {
            // Arrange
            _services.AddSingleton<IBonWorkerManager, CustomWorkerManager>();

            // Act
            _configurationContext.AddWorkers(config =>
            {
                config.RegisterWorker<TestWorkerA>();
            });

            // Build the service provider
            var serviceProvider = _services.BuildServiceProvider();

            // Assert
            var workerManager = serviceProvider.GetService<IBonWorkerManager>();
            Assert.NotNull(workerManager);
            Assert.IsType<CustomWorkerManager>(workerManager);
        }
    }
}
