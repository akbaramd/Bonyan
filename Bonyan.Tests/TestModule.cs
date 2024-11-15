using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;

namespace Bonyan.Tests
{
    public class TestModule : BonModule
    {
        public override Task OnConfigureAsync(BonConfigurationContext context)
        {
            // Custom configuration logic for testing
            return Task.CompletedTask;
        }
    }
}