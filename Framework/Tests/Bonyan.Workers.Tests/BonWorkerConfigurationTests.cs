using System.Reflection;
using Bonyan.Modularity;
using Bonyan.Workers.Tests.Mock;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Workers.Tests
{
    public class BonWorkerConfigurationTests
    {
        private readonly IServiceCollection _services;
        private readonly BonPostConfigurationContext _context;

        public BonWorkerConfigurationTests()
        {
            _services = new ServiceCollection();
            
            _context = new BonPostConfigurationContext(_services);
        }

        [Fact]
        public void RegisterWorkersFromAssemblies_Should_Register_Worker_Types()
        {
            // Arrange
            var workerConfig = new BonWorkerConfiguration(_context);
            var assembly = Assembly.GetExecutingAssembly();

            // Act
            workerConfig.RegisterWorkersFromAssemblies(assembly);

            // Assert
            var registeredWorkers = workerConfig.GetRegisteredWorkerTypes().ToList();

            Assert.Contains(typeof(TestWorkerA), registeredWorkers);
            Assert.Contains(typeof(TestWorkerB), registeredWorkers);
        }

        [Fact]
        public void RegisterWorkersFromTypes_Should_Register_Specified_Worker_Types()
        {
            // Arrange
            var workerConfig = new BonWorkerConfiguration(_context);

            // Act
            workerConfig.RegisterWorkersFromTypes(typeof(TestWorkerA), typeof(TestWorkerB));

            // Assert
            var registeredWorkers = workerConfig.GetRegisteredWorkerTypes().ToList();

            Assert.Contains(typeof(TestWorkerA), registeredWorkers);
            Assert.Contains(typeof(TestWorkerB), registeredWorkers);
        }

        [Fact]
        public void RegisterWorker_Should_Register_Specific_Worker_Type()
        {
            // Arrange
            var workerConfig = new BonWorkerConfiguration(_context);

            // Act
            workerConfig.RegisterWorker<TestWorkerA>();

            // Assert
            var registeredWorkers = workerConfig.GetRegisteredWorkerTypes().ToList();

            Assert.Contains(typeof(TestWorkerA), registeredWorkers);
        }


        [Fact]
        public void RegisterWorker_Should_Not_Register_Duplicate_Types()
        {
            // Arrange
            var workerConfig = new BonWorkerConfiguration(_context);

            // Act
            workerConfig.RegisterWorker<TestWorkerA>();
            workerConfig.RegisterWorker<TestWorkerA>(); // Duplicate registration

            // Assert
            var registeredWorkers = workerConfig.GetRegisteredWorkerTypes().ToList();

            Assert.Single(registeredWorkers);
            Assert.Contains(typeof(TestWorkerA), registeredWorkers);
        }

        [Fact]
        public void RegisterWorkersFromAssemblies_Should_Throw_On_Null_Input()
        {
            // Arrange
            var workerConfig = new BonWorkerConfiguration(_context);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => workerConfig.RegisterWorkersFromAssemblies(null));
        }

        [Fact]
        public void RegisterWorkersFromTypes_Should_Throw_On_Null_Input()
        {
            // Arrange
            var workerConfig = new BonWorkerConfiguration(_context);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => workerConfig.RegisterWorkersFromTypes(null));
        }

        [Fact]
        public void RegisterWorkersFromTypes_Should_Ignore_NonWorker_Types()
        {
            // Arrange
            var workerConfig = new BonWorkerConfiguration(_context);

            // Act
            workerConfig.RegisterWorkersFromTypes(typeof(string)); // string is not an IBonWorker

            // Assert
            var registeredWorkers = workerConfig.GetRegisteredWorkerTypes().ToList();
            Assert.Empty(registeredWorkers);
        }

        [Fact]
        public void RegisterWorker_Should_Respect_Specified_Lifetime()
        {
            // Arrange
            var workerConfig = new BonWorkerConfiguration(_context, ServiceLifetime.Scoped);

            // Act
            workerConfig.RegisterWorker<TestWorkerA>();

            // Assert
            var descriptor = _services.FirstOrDefault(sd => sd.ServiceType == typeof(TestWorkerA));
            Assert.NotNull(descriptor);
            Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
        }

        [Fact]
        public void GetRegisteredWorkerTypes_Should_Return_Registered_Types()
        {
            // Arrange
            var workerConfig = new BonWorkerConfiguration(_context);

            // Act
            workerConfig.RegisterWorker<TestWorkerA>();
            workerConfig.RegisterWorker<TestWorkerB>();

            var registeredWorkers = workerConfig.GetRegisteredWorkerTypes().ToList();

            // Assert
            Assert.Equal(2, registeredWorkers.Count);
            Assert.Contains(typeof(TestWorkerA), registeredWorkers);
            Assert.Contains(typeof(TestWorkerB), registeredWorkers);
        }
    }
}
