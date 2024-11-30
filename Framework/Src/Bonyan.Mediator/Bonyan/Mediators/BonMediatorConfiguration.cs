using Bonyan.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Mediators
{
    /// <summary>
    /// Provides configuration options for registering message consumers and dispatchers in the messaging system.
    /// </summary>
    public class BonMediatorConfiguration
    {
        public BonMediatorConfiguration(BonConfigurationContext services)
        {
            Context = services ?? throw new ArgumentNullException(nameof(services));
          

            // Register the configuration instance
            Context.Services.AddSingleton(this);
        }

        public BonConfigurationContext Context { get; set; }

     
    }
}
