using System.Reflection;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Bonyan.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Bonyan.Tests.Modularity
{
    public class BonModularityApplicationTests
    {
  




       
        [Fact]
        public void Constructor_Should_Throw_If_ServiceCollection_Is_Null()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new BonModularityApplication<TestModule>(null, "service-key", "Service Title"));
        }

        [Fact]
        public void Constructor_Should_Register_Core_Services()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var application = new BonModularityApplication<TestModule>(services, "service-key", "Service Title");

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            Assert.NotNull(serviceProvider.GetService<IBonModuleLoader>());
            Assert.NotNull(serviceProvider.GetService<IBonModuleContainer>());
            Assert.NotNull(serviceProvider.GetService<IBonModuleConfigurator>());
            Assert.NotNull(serviceProvider.GetService<IBonModuleInitializer>());
            Assert.NotNull(serviceProvider.GetService<IBonModularityApplication>());
        }

        // Helper method to replace module instance in the application
        private void ReplaceModuleInstance(BonModularityApplication<TestModule> application, IBonModule moduleInstance)
        {
            var bonModuleAccessorField = typeof(BonModularityApplication<TestModule>)
                .GetField("_bonModuleLoader", BindingFlags.NonPublic | BindingFlags.Instance);
            var bonModuleAccessor = (IBonModuleLoader)bonModuleAccessorField.GetValue(application);

            var modulesField = typeof(BonModuleLoader)
                .GetField("_modules", BindingFlags.NonPublic | BindingFlags.Instance);
            var modules = (Dictionary<Type, BonModuleDescriptor>)modulesField.GetValue(bonModuleAccessor);

            var moduleType = moduleInstance.GetType();
            if (modules.ContainsKey(moduleType))
            {
                modules[moduleType].Instance = moduleInstance;
            }
            else
            {
                var moduleInfo = new BonModuleDescriptor(moduleType, moduleInstance, false, "service-key");
                modules[moduleType] = moduleInfo;
            }
        }
    }
}
