using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;

namespace Bonyan.Tests
{
    public class TestModule : BonModule
    {
        public override ValueTask OnConfigureAsync(BonConfigurationContext context , CancellationToken cancellationToken = default)
        {
            // Custom configuration logic for testing
            return ValueTask.CompletedTask;
        }
    }
}