using System;
using System.Collections.Generic;
using System.Linq;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Xunit;

namespace Bonyan.Tests.Modularity
{
    public class ModuleManagerTests
    {
        [Fact]
        public void LoadModules_Should_Load_Main_Module()
        {
            // Arrange
            var accessor = new BonModuleAccessor();
            var manager = new ModuleManager(accessor);

            // Act
            manager.LoadModules(typeof(TestModule));

            // Assert
            var modules = accessor.GetAllModules().ToList();
            Assert.Contains(modules, m => m.ModuleType == typeof(TestModule));
        }

        [Fact]
        public void LoadModules_Should_Load_Dependent_Modules()
        {
            // Arrange
            var accessor = new BonModuleAccessor();
            var manager = new ModuleManager(accessor);

            // Act
            manager.LoadModules(typeof(DependentTestModule));

            // Assert
            var modules = accessor.GetAllModules().ToList();
            Assert.Contains(modules, m => m.ModuleType == typeof(DependentTestModule));
            Assert.Contains(modules, m => m.ModuleType == typeof(TestModule));
        }

        [Fact]
        public void LoadModules_Should_Throw_On_Circular_Dependency()
        {
            // Arrange
            var accessor = new BonModuleAccessor();
            var manager = new ModuleManager(accessor);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => manager.LoadModules(typeof(CircularModule)));
        }

        [Fact]
        public void ValidateModuleType_Should_Throw_On_Invalid_Type()
        {
            // Arrange
            var invalidModuleType = typeof(string);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => ModuleManager.ValidateModuleType(invalidModuleType));
        }

        [Fact]
        public void LoadModules_Should_Load_Modules_In_Correct_Order()
        {
            // Arrange
            var accessor = new BonModuleAccessor();
            var manager = new ModuleManager(accessor);

            // Act
            manager.LoadModules(typeof(DependentTestModule));

            // Assert
            var modules = accessor.GetAllModules().ToList();
            var dependencyIndex = modules.FindIndex(m => m.ModuleType == typeof(TestModule));
            var mainModuleIndex = modules.FindIndex(m => m.ModuleType == typeof(DependentTestModule));

            Assert.True(dependencyIndex < mainModuleIndex);
        }

        // Circular dependency modules for testing
        
    }
}
