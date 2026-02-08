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

        public override ValueTask OnConfigureAsync(BonConfigurationContext context , CancellationToken cancellationToken = default)
        {
            // Custom configuration logic for testing
            return ValueTask.CompletedTask;
        }
    }
}