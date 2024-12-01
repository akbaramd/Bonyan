using Bonyan.Castle.DynamicProxy;
using Bonyan.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Tests.Modularity
{
    public class BonMasterModuleTests
    {
        private readonly IServiceCollection _services;

        public BonMasterModuleTests()
        {
            _services = new ServiceCollection();
        }

        [Fact]
        public void BonMasterModule_Should_Register_Required_Services()
        {
            // Arrange
            var application = new BonModularityApplication<TestModule>(_services,"service");


            application.ConfigureModulesAsync().GetAwaiter();
            
            // Act
            // Build the service provider to test registrations
            var serviceProvider = _services.BuildServiceProvider();

            // Assert
            // Check that IObjectAccessor<IServiceProvider> is registered
            var objectAccessor = serviceProvider.GetService<IBonObjectAccessor<IServiceProvider>>();
            Assert.NotNull(objectAccessor);

            // Check that BonAsyncDeterminationInterceptor<> is registered
            var interceptorDescriptor = _services.FirstOrDefault(s =>
                s.ServiceType.IsGenericType &&
                s.ServiceType.GetGenericTypeDefinition() == typeof(BonAsyncDeterminationInterceptor<>));

            Assert.NotNull(interceptorDescriptor);
            Assert.Equal(ServiceLifetime.Transient, interceptorDescriptor.Lifetime);

            // Check that IBonCachedServiceProviderBase is registered as BonLazyServiceProvider
            var cachedServiceProvider = serviceProvider.GetService<IBonCachedServiceProviderBase>();
            Assert.NotNull(cachedServiceProvider);
            Assert.IsType<BonLazyServiceProvider>(cachedServiceProvider);

            // Check that IBonLazyServiceProvider is registered as BonLazyServiceProvider
            var lazyServiceProvider = serviceProvider.GetService<IBonLazyServiceProvider>();
            Assert.NotNull(lazyServiceProvider);
            Assert.IsType<BonLazyServiceProvider>(lazyServiceProvider);
        }

        [Fact]
        public async Task BonMasterModule_Configuration_Should_Succeed()
        {
            // Arrange
            var application = new BonModularityApplication<TestModule>(_services,"service");

            // Act
            var configureTask = application.ConfigureModulesAsync();

            // Assert
            await configureTask; // Should complete without exceptions
        }
    }

    // Dummy TestModule for initializing the application
  
}
