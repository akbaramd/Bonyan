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
        public async Task InitializeModulesAsync_Should_Invoke_Module_Initialize_Methods()
        {
            // Arrange
            var services = new ServiceCollection();
            
            var application = new BonModularityApplication<TestModule>(services,"servicename");

            var moduleMock = new Mock<TestModule> { CallBase = true };
            moduleMock.Setup(m => m.OnPreInitializeAsync(It.IsAny<BonInitializedContext>())).Returns(Task.CompletedTask);
            moduleMock.Setup(m => m.OnInitializeAsync(It.IsAny<BonInitializedContext>())).Returns(Task.CompletedTask);
            moduleMock.Setup(m => m.OnPostInitializeAsync(It.IsAny<BonInitializedContext>())).Returns(Task.CompletedTask);

            ReplaceModuleInstance(application, moduleMock.Object);

            
            var serviceProvider = services.BuildServiceProvider();// Act
            await application.InitializeModulesAsync(serviceProvider);

            // Assert
            moduleMock.Verify(m => m.OnPreInitializeAsync(It.IsAny<BonInitializedContext>()), Times.Once);
            moduleMock.Verify(m => m.OnInitializeAsync(It.IsAny<BonInitializedContext>()), Times.Once);
            moduleMock.Verify(m => m.OnPostInitializeAsync(It.IsAny<BonInitializedContext>()), Times.Once);
        }



       
        [Fact]
        public void Constructor_Should_Throw_If_ServiceCollection_Is_Null()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new BonModularityApplication<TestModule>(null,"servicename"));
        }

        [Fact]
        public void Constructor_Should_Register_Core_Services()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var application = new BonModularityApplication<TestModule>(services,"service");

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
                var moduleInfo = new BonModuleDescriptor(moduleType,moduleInstance,false);
                modules[moduleType] = moduleInfo;
            }
        }
    }
}
