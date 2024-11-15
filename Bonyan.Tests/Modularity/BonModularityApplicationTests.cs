using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Bonyan.Tests.Modularity
{
    public class BonModularityApplicationTests
    {
        [Fact]
        public async Task ConfigureModulesAsync_Should_Invoke_Module_Configure_Methods()
        {
            // Arrange
            var services = new ServiceCollection();
            var application = new BonModularityApplication<TestModule>(services);

            var moduleMock = new Mock<TestModule> { CallBase = true };
            moduleMock.Setup(m => m.OnPreConfigureAsync(It.IsAny<BonConfigurationContext>())).Returns(Task.CompletedTask);
            moduleMock.Setup(m => m.OnConfigureAsync(It.IsAny<BonConfigurationContext>())).Returns(Task.CompletedTask);
            moduleMock.Setup(m => m.OnPostConfigureAsync(It.IsAny<BonConfigurationContext>())).Returns(Task.CompletedTask);

            ReplaceModuleInstance(application, moduleMock.Object);

            // Act
            await application.ConfigureModulesAsync();

            // Assert
            moduleMock.Verify(m => m.OnPreConfigureAsync(It.IsAny<BonConfigurationContext>()), Times.Once);
            moduleMock.Verify(m => m.OnConfigureAsync(It.IsAny<BonConfigurationContext>()), Times.Once);
            moduleMock.Verify(m => m.OnPostConfigureAsync(It.IsAny<BonConfigurationContext>()), Times.Once);
        }

        [Fact]
        public async Task InitializeModulesAsync_Should_Invoke_Module_Initialize_Methods()
        {
            // Arrange
            var services = new ServiceCollection();
            
            var application = new BonModularityApplication<TestModule>(services);

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
        public async Task ConfigureModulesAsync_Should_Handle_Exceptions()
        {
            // Arrange
            var services = new ServiceCollection();
            var application = new BonModularityApplication<TestModule>(services);

            var moduleMock = new Mock<TestModule> { CallBase = true };
            moduleMock.Setup(m => m.OnPreConfigureAsync(It.IsAny<BonConfigurationContext>())).ThrowsAsync(new Exception("Test exception"));

            ReplaceModuleInstance(application, moduleMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => application.ConfigureModulesAsync());
        }

        [Fact]
        public async Task InitializeModulesAsync_Should_Handle_Exceptions()
        {
            // Arrange
            var services = new ServiceCollection();
            var serviceProvider = services.BuildServiceProvider();
            var application = new BonModularityApplication<TestModule>(services);

            var moduleMock = new Mock<TestModule> { CallBase = true };
            moduleMock.Setup(m => m.OnPreInitializeAsync(It.IsAny<BonInitializedContext>())).ThrowsAsync(new Exception("Test exception"));

            ReplaceModuleInstance(application, moduleMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => application.InitializeModulesAsync(serviceProvider));
        }

        [Fact]
        public void Constructor_Should_Throw_If_ServiceCollection_Is_Null()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new BonModularityApplication<TestModule>(null));
        }

        [Fact]
        public void Constructor_Should_Register_Core_Services()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var application = new BonModularityApplication<TestModule>(services);

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            Assert.NotNull(serviceProvider.GetService<IBonModuleAccessor>());
            Assert.NotNull(serviceProvider.GetService<IModuleLoader>());
            Assert.NotNull(serviceProvider.GetService<IBonModuleConfigurator>());
            Assert.NotNull(serviceProvider.GetService<IBonModuleInitializer>());
            Assert.NotNull(serviceProvider.GetService<IBonModularityApplication>());
        }

        // Helper method to replace module instance in the application
        private void ReplaceModuleInstance(BonModularityApplication<TestModule> application, IBonModule moduleInstance)
        {
            var bonModuleAccessorField = typeof(BonModularityApplication<TestModule>)
                .GetField("_bonModuleAccessor", BindingFlags.NonPublic | BindingFlags.Instance);
            var bonModuleAccessor = (IBonModuleAccessor)bonModuleAccessorField.GetValue(application);

            var modulesField = typeof(BonModuleAccessor)
                .GetField("_modules", BindingFlags.NonPublic | BindingFlags.Instance);
            var modules = (Dictionary<Type, ModuleInfo>)modulesField.GetValue(bonModuleAccessor);

            var moduleType = moduleInstance.GetType();
            if (modules.ContainsKey(moduleType))
            {
                modules[moduleType].Instance = moduleInstance;
            }
            else
            {
                var moduleInfo = new ModuleInfo(moduleType) { Instance = moduleInstance };
                modules[moduleType] = moduleInfo;
            }
        }
    }
}
