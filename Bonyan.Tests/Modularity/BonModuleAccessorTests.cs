using System;
using System.Collections.Generic;
using System.Linq;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Xunit;

namespace Bonyan.Tests.Modularity
{
    public class BonModuleAccessorTests
    {
        [Fact]
        public void AddModule_Should_Add_Module()
        {
            // Arrange
            var accessor = new BonModuleAccessor();
            var moduleType = typeof(TestModule);
            var moduleInfo = new ModuleInfo(moduleType);

            // Act
            accessor.AddModule(moduleInfo);

            // Assert
            var retrievedModule = accessor.GetModule(moduleType);
            Assert.NotNull(retrievedModule);
            Assert.Equal(moduleInfo, retrievedModule);
        }

        [Fact]
        public void GetModule_Should_Return_Null_If_Not_Found()
        {
            // Arrange
            var accessor = new BonModuleAccessor();
            var moduleType = typeof(TestModule);

            // Act
            var retrievedModule = accessor.GetModule(moduleType);

            // Assert
            Assert.Null(retrievedModule);
        }

        [Fact]
        public void ClearModules_Should_Remove_All_Modules()
        {
            // Arrange
            var accessor = new BonModuleAccessor();
            var moduleType = typeof(TestModule);
            var moduleInfo = new ModuleInfo(moduleType);
            accessor.AddModule(moduleInfo);

            // Act
            accessor.ClearModules();

            // Assert
            var allModules = accessor.GetAllModules();
            Assert.Empty(allModules);
        }

        [Fact]
        public void GetAllModules_Should_Return_All_Added_Modules()
        {
            // Arrange
            var accessor = new BonModuleAccessor();
            var moduleType1 = typeof(TestModule);
            var moduleType2 = typeof(DependentTestModule);
            var moduleInfo1 = new ModuleInfo(moduleType1);
            var moduleInfo2 = new ModuleInfo(moduleType2);

            accessor.AddModule(moduleInfo1);
            accessor.AddModule(moduleInfo2);

            // Act
            var allModules = accessor.GetAllModules();

            // Assert + 1 default modules
            Assert.Equal(3, allModules.Count());
            Assert.Contains(moduleInfo1, allModules);
            Assert.Contains(moduleInfo2, allModules);
        }

        [Fact]
        public void AddModule_Should_Update_Module_If_Already_Exists()
        {
            // Arrange
            var accessor = new BonModuleAccessor();
            var moduleType = typeof(TestModule);
            var moduleInfo1 = new ModuleInfo(moduleType);
            var moduleInfo2 = new ModuleInfo(moduleType);

            accessor.AddModule(moduleInfo1);

            // Act
            accessor.AddModule(moduleInfo2);

            // Assert
            var retrievedModule = accessor.GetModule(moduleType);
            Assert.NotNull(retrievedModule);
            Assert.Equal(moduleInfo2, retrievedModule);
        }
    }
}
