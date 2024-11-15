using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;

namespace Bonyan.Tests
{
    public class DependentTestModule : BonModule
    {

        // Add a dependency on TestModule
        public DependentTestModule()
        {
            DependOn<TestModule>();
        }

        public override Task OnConfigureAsync(BonConfigurationContext context)
        {
            // Custom configuration logic for testing
            return Task.CompletedTask;
        }
    }
}